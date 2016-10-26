using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditScoreEnricher
{
    public class Reciever
    {

        public void Recieve(ConnectionFactory factory)
        {

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "helloper", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    message = Enricher.Enrich(message);
                    new Sender().Send(factory, message);
                };
                channel.BasicConsume(queue: "helloper", noAck: true, consumer: consumer);

                

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

        }

    }
}
