using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Normalizer
{
    class Program
    {
        public static void Main()
        {
            string HOSTNAME = "datdb.cphbusiness.dk";
            string USERNAME = "student";
            string PASSWORD = "cph";

            string XMLBANK = "cphbusiness.bankXML";
            string JSONBANK = "cphbusiness.bankJSON";
            string BANKQUEUE = "laurbaer_norm";
            string RESULTROUTINGKEY = "NormalizedKEY";
            string RESULTEXCHANGE = "laurbaer_aggr";

            var factory = new ConnectionFactory() { HostName = HOSTNAME, UserName = USERNAME, Password = PASSWORD };
            var connection = factory.CreateConnection();
            var XMLchannel = connection.CreateModel();
            var JSONchannel = connection.CreateModel();
            var resultChannel = connection.CreateModel();

            XMLchannel.QueueDeclare(queue: BANKQUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
            JSONchannel.QueueDeclare(queue: BANKQUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
           
            var XMLqueueName = XMLchannel.QueueDeclare().QueueName;
            var JSONqueueName = JSONchannel.QueueDeclare().QueueName;

            XMLchannel.QueueBind(queue: BANKQUEUE, exchange: XMLBANK, routingKey: "");
            JSONchannel.QueueBind(queue: BANKQUEUE, exchange: JSONBANK, routingKey: "");
            
            //XML Consumer - When xml messages arrives, this EventBasicConsumer will handle it.
            var XMLconsumer = new EventingBasicConsumer(XMLchannel);
            XMLconsumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        try
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(LoanRequest));
                            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(message));
                            LoanRequest tempLR = (LoanRequest)serializer.Deserialize(memStream);
                            JObject resultingMessage = JObject.FromObject(tempLR);
                            resultingMessage.Add("bank", XMLBANK);

                            Console.WriteLine("Received: {0}", resultingMessage.ToString());

                            var resultBody = Encoding.UTF8.GetBytes(resultingMessage.ToString());

                            resultChannel.BasicPublish(exchange: RESULTEXCHANGE, routingKey: RESULTROUTINGKEY, basicProperties: null, body: resultBody);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(message);
                            Console.WriteLine(e.StackTrace);
                        }
                    };

            //JSON Consumer - When JSON messages arrives, this EventBasicConsumer will handle it.
            var JSONconsumer = new EventingBasicConsumer(JSONchannel);
            JSONconsumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        string message = Encoding.UTF8.GetString(body);

                        JObject messageJObject = JObject.Parse(message);
                        messageJObject.Add("bank", JSONBANK);

                        Console.WriteLine("Received from JSON: {0}", messageJObject.ToString());

                        var resultBody = Encoding.UTF8.GetBytes(messageJObject.ToString());

                       resultChannel.BasicPublish(exchange: RESULTEXCHANGE, routingKey: RESULTROUTINGKEY, basicProperties: null, body: resultBody);
                    };

            var consumerChannel1 = connection.CreateModel();
            var consumerChannel2 = connection.CreateModel();



            consumerChannel1.BasicConsume(queue: BANKQUEUE, noAck: true, consumer: JSONconsumer);
            consumerChannel2.BasicConsume(queue: BANKQUEUE, noAck: true, consumer: XMLconsumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
               

        }
    }
}