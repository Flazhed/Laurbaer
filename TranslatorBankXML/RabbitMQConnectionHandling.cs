using System;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TranslatorBankXML
{
    class RabbitMQConnectionHandling
    {
        private ConnectionFactory factory;
        private static string _Queue_Name = "Hello";
        private static string _Host_Name = "localhost";
        public RabbitMQConnectionHandling()
        {
            factory = new ConnectionFactory() { HostName = _Host_Name };
        }
        public string readQueue()
        {
            string message = null;
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _Queue_Name,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    };
                    channel.BasicConsume(queue: _Queue_Name,
                                         noAck: true,
                                         consumer: consumer);

                }
            }
            return message;
        }

        public void SendXMLToBankQueue(string XMLBankFormat)
        {
            var factory = new ConnectionFactory() { HostName = _Host_Name };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _Queue_Name, durable: false, exclusive: false, autoDelete: false, arguments: null);

                string message = XMLBankFormat;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }
    }
}
