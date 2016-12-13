using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankEnricher.src
{
    public class Sender
    {

        public static void Send(ConnectionFactory factory, string message, IBasicProperties prob)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.ExchangeDeclare(exchange: Constants.DirectExchanceName, type: Constants.DirectExchanceType);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: Constants.DirectExchanceName, routingKey: Constants.DirectRoutingKeyEnriched, basicProperties: prob, body: body);
                Console.WriteLine(" [x] Sent {0}", message);



            }

        }
    }
    }

