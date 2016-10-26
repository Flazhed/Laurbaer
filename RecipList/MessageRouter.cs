using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;

namespace RecipList
{
    public class MessageRouter
    {
        private readonly ConnectionFactory _factory;

        public MessageRouter(ConnectionFactory factory)
        {
            _factory = factory;
        }

        public void SendToRecipientList(char[] msg, List<Bank> lendersList)
        {
            var body = Encoding.UTF8.GetBytes(msg);
            using (var connection = _factory.CreateConnection())

                foreach (var lender in lendersList)
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: lender.queue,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        channel.BasicPublish(exchange: Constants.ExhangeType,
                            routingKey: "",
                            body: body);
                    }
                }
        }
    }
}