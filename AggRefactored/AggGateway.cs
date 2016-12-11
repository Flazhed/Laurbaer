//using System;
//using System.Collections.Generic;
//using System.Text;
//using Aggregator;
//using Aggregator.Entity;
//using AggRefactored.Interfaces;
//using Newtonsoft.Json;
//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;

//namespace AggRefactored
//{
//    public class AggGateway : IMessaging
//    {
//        private readonly ConnectionFactory _factory;
//        private Dictionary<string, BankQuoteAggregate> aggregators;

//        public AggGateway(ConnectionFactory factory)
//        {
//            _factory = factory;
//            aggregators = new Dictionary<string, BankQuoteAggregate>();
//        }

//        public void Sender()
//        {
//            var connection = _factory.CreateConnection();
//            var channel = connection.CreateModel();

//            channel.ExchangeDeclare(exchange: "laurbaer_direct", type: "Direct");
//            Dictionary<string, object> args = new Dictionary<string, object>();
//            args.Add("x-dead-letter-exchange", "dead_exchange");
//            var queueName = channel.QueueDeclare("laur_agg", false, false, false, args).QueueName;
//            channel.QueueBind(queue: queueName, exchange: "laurbaer_direct", routingKey: "aggOut");

//            var consumer = new EventingBasicConsumer(channel);
//            consumer.Received += (model, ea) =>
//            {
//                var body = ea.Body;
//                var probs = ea.BasicProperties;
//                var message = Encoding.UTF8.GetString(body);
//                Console.ForegroundColor = ConsoleColor.DarkGreen;
//                Console.WriteLine("[{0}] << received on {1}",
//                    DateTime.Now.ToString("HH:mm:ss"), message);

//                LoanRequest loanRequest = null;
//                try
//                {
//                    loanRequest = JsonConvert.DeserializeObject<LoanRequest>(message);
//                }
//                catch (JsonException ex)
//                {
//                    Console.WriteLine(ex.Message);
//                    Console.WriteLine("Sending to Dead letter chan.");
//                    channel.BasicNack(ea.DeliveryTag, false, false);
//                    //Todo: Send to dead letter
//                    return;
//                }
//                if (probs.CorrelationId != null)
//                {
//                    Console.WriteLine("Ack");
//                    channel.BasicAck(ea.DeliveryTag, true);
//                    aggregators.Add(probs.CorrelationId,
//                        new BankQuoteAggregate(loanRequest.count, _messageRouter));
//                }
//                else
//                {
//                    Console.WriteLine();
//                    Console.WriteLine("NoAck - " + ea.DeliveryTag);
//                    channel.BasicNack(ea.DeliveryTag, false, false);
//                }
//            };
//            channel.BasicConsume(queue: queueName,
//                noAck: false,
//                consumer: consumer);
//        }

//        public void Consumer()
//        {
//            throw new System.NotImplementedException();
//        }

//        public void deadLetterHandler()
//        {
//        }
//    }
//}