using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Aggregator.Entity;
using Aggregator.Routes;
using Aggregator.Utils;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace Aggregator
{
    public class BankGateway
    {
        private Dictionary<string, BankQuoteAggregate> aggregators;
        private readonly ConnectionFactory _factory;
        private readonly MessageRouter _messageRouter;
        private string whatever { set; get; }

        private int counter = 0;
        public BankGateway(ConnectionFactory factory, MessageRouter messageRouter)
        {
            _factory = factory;
            _messageRouter = messageRouter;
            aggregators = new Dictionary<string, BankQuoteAggregate>();
        }

        public void CreateConsumer()
        {
            var counter = 0;

            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "laurbaer_direct", type: Constants.DirectExhangeType);
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("x-dead-letter-exchange", "dead_exchange");
            var queueName = channel.QueueDeclare("laur_agg", false, false, false, args).QueueName;
            channel.QueueBind(queue: queueName, exchange: "laurbaer_direct", routingKey: "aggOut");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                counter++;
                var body = ea.Body;
                var probs = ea.BasicProperties;
                var message = Encoding.UTF8.GetString(body);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("[{0}] << received on {1}",
                    DateTime.Now.ToString("HH:mm:ss"), message);

                LoanRequest loanRequest = null;
                try
                {
                    loanRequest = JsonConvert.DeserializeObject<LoanRequest>(message);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Sending to Dead letter chan.");
                    channel.BasicNack(ea.DeliveryTag, false, false);
                    //Todo: Send to dead letter
                    return;
                }
                if (probs.CorrelationId != null)
                {
                    Console.WriteLine("Ack");
                    channel.BasicAck(ea.DeliveryTag, true);
                    aggregators.Add(probs.CorrelationId,
                        new BankQuoteAggregate(loanRequest.count, _messageRouter));
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("NoAck - " + ea.DeliveryTag);
                    channel.BasicNack(ea.DeliveryTag, false, false);
                   
                }
            };
            channel.BasicConsume(queue: queueName,
                noAck: false,
                consumer: consumer);
        }

        public void CreateSecondConsumer()
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: Constants.DirectExchangeName, type: Constants.DirectExhangeType);

            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("x-dead-letter-exchange", "dead_exchange");

            var queueName = channel.QueueDeclare("", false, false, false, args).QueueName;
            Console.WriteLine(queueName);
            channel.QueueBind(queue: queueName, exchange: Constants.DirectExchangeName, routingKey: "laurbaer_aggr");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var probs = ea.BasicProperties;
                var message = Encoding.UTF8.GetString(body);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("[{0}] << received on {1}",
                    DateTime.Now.ToString("HH:mm:ss"), message);

                

                BankReply loanRequest = null;
                try
                {
                    loanRequest = JsonConvert.DeserializeObject<BankReply>(message);
                }
                catch (JsonException ex)
                {
                    Dictionary<string, Object> err = new Dictionary<string, Object>();
                    err.Add("test", ex.Message);

                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Sending to Dead letter chan.");
                    ea.Body = Encoding.ASCII.GetBytes(ex.Message);
                    channel.BasicNack(ea.DeliveryTag, false, false);
                    //Todo: Send to dead letter
                    return;
                }
                
                //If no correlationId found.. Dead letter channel or whatevr. also handle timeout
                BankQuoteAggregate bankQuoteAggregate = null;
                if (aggregators.TryGetValue(probs.CorrelationId, out bankQuoteAggregate))
                {
                    channel.BasicAck(ea.DeliveryTag, false);
                    bankQuoteAggregate.AddBankReply(loanRequest);
                }
                else
                {
                    //channel.BasicAck(ea.DeliveryTag, false);
                    //channel.BasicReject(ea.DeliveryTag, false);
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }


                // Err channel here.
            };
            channel.BasicConsume(queue: queueName,
                noAck: false,
                consumer: consumer);
        }

        public void CreateDeadLetterWkr()
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("dead_exchange", "fanout");


            channel.QueueDeclare("dead_queue", false, false, false, null);
            channel.QueueBind(queue: "dead_queue", exchange: "dead_exchange", routingKey: "");


            //var consumer = new EventingBasicConsumer(channel);
            //consumer.Received += (model, ea) =>
            //{
            //    var body = ea.Body;
            //    var probs = ea.BasicProperties;
            //    var message = Encoding.UTF8.GetString(body);
            //    Console.WriteLine("In DL msg: {0}", message);
            //};
            //channel.BasicConsume(queue: "dead_queue", consumer: consumer);
        }
    }
}