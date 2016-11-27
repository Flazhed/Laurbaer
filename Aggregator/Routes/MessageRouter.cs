using System;
using System.Text;
using Aggregator.Entity;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Aggregator.Utils;

namespace Aggregator.Routes
{
    public class MessageRouter
    {
        private readonly ConnectionFactory _factory;

        public MessageRouter(ConnectionFactory factory)
        {
            _factory = factory;
        }

        public void SendToRecipientList(BankReply bankReply)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                string routingKey = "Should not be hardcoded here.";

                string jsonRecip = JsonConvert.SerializeObject(bankReply, Formatting.Indented);

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("[{0}] >> Sending to {1}",
                    DateTime.Now.ToString("HH:mm:ss"), routingKey);

                channel.ExchangeDeclare(exchange: Constants.DirectExchangeName,
                    type: Constants.DirectExhangeType);

                var body = Encoding.UTF8.GetBytes(jsonRecip);
                channel.BasicPublish(exchange: Constants.DirectExchangeName,
                    routingKey: routingKey,
                    basicProperties: null,
                    body: body);
            }
        }

    }
}