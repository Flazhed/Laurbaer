using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
                channel.ExchangeDeclare("laurbaer_direct", "direct");

                string quName = channel.QueueDeclare().QueueName;

                channel.QueueBind(quName, "laurbaer_direct", "laurbaer_soap_response");

                QueueingBasicConsumer consumer;
                //           channel.BasicQos(0, 1, false);
                try
                {
                    consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(quName, true, consumer);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    return "Error:" + exception;
                }

                string response = null;
                var ea = (BasicDeliverEventArgs) consumer.Queue.Dequeue();

                var body = ea.Body;
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    message = Encoding.UTF8.GetString(body);
                }
                catch (Exception e)
                {
                    Console.WriteLine(" [.] " + e.Message);
                    response = "";
                }
            }
            return message;
        }
    }
}