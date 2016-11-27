using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecipList.Entity;

namespace RecipList
{
    public class Worker
    {
        private readonly ConnectionFactory _factory;
        private readonly MessageRouter _messageRouter;


        public Worker(ConnectionFactory factory, MessageRouter messageRouter)
        {
            _factory = factory;
            _messageRouter = messageRouter;
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
                var message = Encoding.UTF8.GetString(body);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("[{0}] << received on {1} - msg: {2}",
                    DateTime.Now.ToString("HH:mm:ss"), Constants.EnricherInRoutingKey, message);

                var loanRequest = JsonConvert.DeserializeObject<LoanRequest>(message);
                var corrId = Guid.NewGuid().ToString();
                _messageRouter.NotifyAggregator(loanRequest, corrId);
                _messageRouter.SendToRecipientList(loanRequest, corrId);
            };

            channel.BasicConsume(queue: queueName,
                noAck: true,
                consumer: consumer);
        }
    }
}