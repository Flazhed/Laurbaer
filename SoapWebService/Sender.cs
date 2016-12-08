using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace SoapWebService
{
    public class Sender
    {
        private ConnectionFactory connectionFactory;
        private const string RoutingKey = "hansen";
        private const string ExchangeName = "laurbaer_direct";
        private string queueName;

        public Sender(string queueName)
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

        public bool Send(LoanRequest loanRequest, string corrId)
        {
            try
            {
                using (var connection = connectionFactory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: ExchangeName, type: "direct");
                    var props = channel.CreateBasicProperties();
                    props.CorrelationId = corrId;
                    props.ReplyTo = queueName;
                    var message = JsonConvert.SerializeObject(loanRequest);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: ExchangeName,
                        routingKey: RoutingKey,
                        basicProperties: props,
                        body: body);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}