using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RecipList.Entity;

namespace RecipList
{
    public class MessageRouter
    {
        private readonly ConnectionFactory _factory;

        public MessageRouter(ConnectionFactory factory)
        {
            _factory = factory;
        }

        public void SendToRecipientList(LoanRequest loanRequest, string corrId)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                for (int i = 0; i < loanRequest.banks.Count; i++)
                {
                    LoanRequest newLoanRequest = new LoanRequest
                    {
                        creditScore = loanRequest.creditScore,
                        loanAmount = loanRequest.loanAmount,
                        loanDuration = loanRequest.loanDuration,
                        ssn = loanRequest.ssn,
                        bank = loanRequest.banks[i]
                    };

                    string routingKey = loanRequest.banks[i].translatorRoutingKey;

                    string jsonRecip = JsonConvert.SerializeObject(newLoanRequest, Formatting.Indented);

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(Constants.DirectExchangeName);
                    Console.WriteLine("[{0}] >> Sending to {1}",
                        DateTime.Now.ToString("HH:mm:ss"), routingKey);

                    channel.ExchangeDeclare(exchange: Constants.DirectExchangeName,type: Constants.DirectExhangeType);

                    var props = channel.CreateBasicProperties();
                    props.CorrelationId = corrId;
                    props.ReplyTo = "laurbaer_norm3";
                    
                   
                    var body = Encoding.UTF8.GetBytes(jsonRecip);
                    channel.BasicPublish(exchange: Constants.DirectExchangeName,
                        routingKey: routingKey,
                        basicProperties: props,
                        body: body);
                }
            }
        }

        public void NotifyAggregator(LoanRequest loanRequest, string corrId)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                LoanRequest newLoanRequest = new LoanRequest
                {
                    ssn = loanRequest.ssn,
                    count = loanRequest.banks.Count
                };

                string jsonNormlizer = JsonConvert.SerializeObject(newLoanRequest, Formatting.Indented);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("[{0}] >> notify to {1}",
                    DateTime.Now.ToString("HH:mm:ss"), Constants.AggregatorOutRoutingKey);

                channel.ExchangeDeclare(exchange: Constants.DirectExchangeName,
                    type: Constants.DirectExhangeType);

                var props = channel.CreateBasicProperties();
                props.CorrelationId = corrId;

                var body = Encoding.UTF8.GetBytes(jsonNormlizer);
                channel.BasicPublish(exchange: Constants.DirectExchangeName,
                    routingKey: Constants.AggregatorOutRoutingKey,
                    basicProperties: props,
                    body: body);
            }
        }
    }
}