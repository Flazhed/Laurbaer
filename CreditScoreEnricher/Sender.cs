using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditScoreEnricher
{
    public class Sender
    {

        public void Send(ConnectionFactory factory, string message)
        {
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.ExchangeDeclare(exchange: Constants.DirectExchanceName, type: Constants.DirectExchanceType);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: Constants.DirectExchanceName, routingKey: Constants.DirectRoutingKeyEnriched, basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent {0}", message);



            }

        }
    }
    }

