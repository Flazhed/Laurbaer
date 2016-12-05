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

            string XMLQUEUE = "laurbaer_norm_xml";
            string JSONQUEUE = "laurbaer_norm_json";
            //string RESULTROUTINGKEY = "someroutingkey.org";
            //string RESULTEXCHANGE = "laurbaer_aggr";

            var factory = new ConnectionFactory() { HostName = HOSTNAME, UserName = USERNAME, Password = PASSWORD };
            var connection = factory.CreateConnection();
            var XMLchannel = connection.CreateModel();
            var JSONchannel = connection.CreateModel();
            var resultChannel = connection.CreateModel();

            XMLchannel.QueueDeclare(queue: XMLQUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
            JSONchannel.QueueDeclare(queue: JSONQUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
           
            //XML Consumer - When xml messages arrives, this EventBasicConsumer will handle it.
            var XMLconsumer = new EventingBasicConsumer(XMLchannel);
            XMLconsumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        try
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(LoanResponse));
                            MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(message));
                            LoanResponse tempLR = (LoanResponse)serializer.Deserialize(memStream);
                            JObject resultingMessage = JObject.FromObject(tempLR);
                            //resultingMessage.Add("bank", XMLBANK);

                            Console.WriteLine("Received: {0}", resultingMessage.ToString());

                            var resultBody = Encoding.UTF8.GetBytes(resultingMessage.ToString());

                           // resultChannel.BasicPublish(exchange: RESULTEXCHANGE, routingKey: RESULTROUTINGKEY, basicProperties: null, body: resultBody);

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Message was: \n");
                            Console.WriteLine(message);
                            Console.WriteLine("ERROR: \n");
                            Console.WriteLine(e.Message);
                        }
                    };

            //JSON Consumer - When JSON messages arrives, this EventBasicConsumer will handle it.
            var JSONconsumer = new EventingBasicConsumer(JSONchannel);
            JSONconsumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        string message = Encoding.UTF8.GetString(body);

                        JObject messageJObject = JObject.Parse(message);
                        //messageJObject.Add("bank", JSONBANK);

                        Console.WriteLine("Received: {0}", messageJObject.ToString());

                        var resultBody = Encoding.UTF8.GetBytes(messageJObject.ToString());

                       //  resultChannel.BasicPublish(exchange: RESULTEXCHANGE, routingKey: RESULTROUTINGKEY, basicProperties: null, body: resultBody);
                    };
            
            JSONchannel.BasicConsume(queue: JSONQUEUE, noAck: true, consumer: JSONconsumer);
            XMLchannel.BasicConsume(queue: XMLQUEUE, noAck: true, consumer: XMLconsumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
               

        }
    }
}