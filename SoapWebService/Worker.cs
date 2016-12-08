using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace SoapWebService
{
    public class Worker
    {
        private ConnectionFactory connectionFactory;
        private string queueName;

        public Worker(string queueName)
        {
            this.queueName = queueName;
            connectionFactory = new ConnectionFactory
            {
                HostName = Constants.Host,
                Port = Constants.Port,
                UserName = Constants.UserName,
                Password = Constants.Password
            };
        }

        public string Consume()
        {
            string message = "";

            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    message = Encoding.UTF8.GetString(body);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: queueName,
                    noAck: false,
                    consumer: consumer);
            }
            return message;
        }
    }
}