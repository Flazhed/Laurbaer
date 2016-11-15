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

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: Constants.DirectExchanceName, type: Constants.DirectExchanceType);

            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: Constants.DirectExchanceName, routingKey: Constants.DirectRoutingKeyReciever);


                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                    message = Enricher.Enrich(message);
                    Console.WriteLine("Enriched");
                    new Sender().Send(factory, message);
                    
                };
                channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
            
            

        }


    }
}
