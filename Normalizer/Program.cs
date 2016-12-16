using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Serialization;
using static Normalizer.Constants;
using static Normalizer.ConnectionFactoryBuilder;
using static Normalizer.Consumer;

namespace Normalizer
{
    class Program
    {
        public static void Main()
        {
            ConnectionFactory factory = CreateConnectionFactory();
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var resultChannel = connection.CreateModel();

            channel.QueueDeclare(queue: QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
            resultChannel.ExchangeDeclare(exchange: RESULTEXCHANGE, type: DIRECTTYPE);

            channel.BasicConsume(queue: QUEUE, noAck: true, consumer: CreateConsumer(channel, resultChannel));

            Console.WriteLine("READY FOR ACTION");
            Console.ReadLine();
        }
    }
}