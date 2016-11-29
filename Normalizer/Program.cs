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

            string XMLBANK = "XMLBankSWAG";
            string JSONBANK = "JSONBankSWAG";
            string RESULTROUTINGKEY = "NormalizedKEY";

            var factory = new ConnectionFactory() { HostName = HOSTNAME, UserName = USERNAME, Password = PASSWORD };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: RESULTROUTINGKEY, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    //XML Consumer - When xml messages arrives, this EventBasicConsumer will handle it.
                    var XMLconsumer = new EventingBasicConsumer(channel);
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

                            channel.BasicPublish(exchange: "", routingKey: RESULTROUTINGKEY, basicProperties: null, body: resultBody);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(message);
                            Console.WriteLine(e.StackTrace);
                        }
                    };
                    //JSON Consumer - When JSON messages arrives, this EventBasicConsumer will handle it.
                    var JSONconsumer = new EventingBasicConsumer(channel);
                    JSONconsumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        string message = Encoding.UTF8.GetString(body);

                        JObject messageJObject = JObject.Parse(message);
                        messageJObject.Add("bank", JSONBANK);

                        Console.WriteLine("Received from JSON: {0}", messageJObject.ToString());

                        var resultBody = Encoding.UTF8.GetBytes(messageJObject.ToString());

                        channel.BasicPublish(exchange: "", routingKey: RESULTROUTINGKEY, basicProperties: null, body: resultBody);
                    };

                    channel.BasicConsume(queue: JSONBANK, noAck: true, consumer: JSONconsumer);
                    channel.BasicConsume(queue: XMLBANK, noAck: true, consumer: XMLconsumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }

        }
    }
}
