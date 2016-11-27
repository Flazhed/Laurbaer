using System;
using System.Collections.Generic;
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


        public BankGateway(ConnectionFactory factory, MessageRouter messageRouter)
        {
            _factory = factory;
            _messageRouter = messageRouter;
            aggregators = new Dictionary<string, BankQuoteAggregate>();
        }

        public void CreateConsumer()
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: Constants.DirectExchangeName, type: Constants.DirectExhangeType);
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: Constants.DirectExchangeName,
                routingKey: Constants.EnricherInRoutingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var probs = ea.BasicProperties;
                var message = Encoding.UTF8.GetString(body);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("[{0}] << received on {1}",
                    DateTime.Now.ToString("HH:mm:ss"), Constants.EnricherInRoutingKey);
                var loanRequest = JsonConvert.DeserializeObject<LoanRequest>(message);
                aggregators.Add(probs.CorrelationId, new BankQuoteAggregate(loanRequest.count));
            };
            channel.BasicConsume(queue: queueName,
                noAck: true,
                consumer: consumer);
        }

        public void CreateSecondConsumer()
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: Constants.DirectExchangeName, type: Constants.DirectExhangeType);
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: Constants.DirectExchangeName,
                routingKey: Constants.EnricherInRoutingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var probs = ea.BasicProperties;
                var message = Encoding.UTF8.GetString(body);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("[{0}] << received on {1}",
                    DateTime.Now.ToString("HH:mm:ss"), Constants.EnricherInRoutingKey);
                var loanRequest = JsonConvert.DeserializeObject<BankReply>(message);

                //If no correlationId found.. Dead letter channel or whatevr. also handle timeout
                BankQuoteAggregate bankQuoteAggregate = null;
                if(aggregators.TryGetValue(probs.CorrelationId, out bankQuoteAggregate))
                {
                    bankQuoteAggregate.AddBankReply(loanRequest);
                }
                // Err channel here.
            };
            channel.BasicConsume(queue: queueName,
                noAck: true,
                consumer: consumer);
        }
    }
}