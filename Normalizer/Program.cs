using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Serialization;
using static Normalizer.Constants;

namespace Normalizer
{
    class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = HOSTNAME, UserName = USERNAME, Password = PASSWORD };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var resultChannel = connection.CreateModel();

            channel.QueueDeclare(queue: QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: null);
            resultChannel.ExchangeDeclare(exchange: RESULTEXCHANGE, type: DIRECTTYPE);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
                    {
                        if(Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"]) != null){
                            var language = (Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"])).ToLower();

                            var body = ea.Body;
                            string message = Encoding.UTF8.GetString(body);

                            if (language.Equals(LANGUAGEJSON))
                            {
                                JObject messageJObject = JObject.Parse(message);

                                Console.WriteLine("Received: {0}", messageJObject.ToString());
                                
                                messageJObject.Add("bankName", JSONBANK);
                                var resultBody = Encoding.UTF8.GetBytes(messageJObject.ToString());

                                Console.WriteLine("Header was: " + Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"]) + "\n");
                                resultChannel.BasicPublish(exchange: RESULTEXCHANGE, routingKey: RESULTROUTINGKEY, basicProperties: ea.BasicProperties, body: resultBody);
                            }
                            else if (language.Equals(LANGUAGEXML))
                            {
                                try
                                {
                                    XmlSerializer serializer = new XmlSerializer(typeof(LoanResponse));
                                    MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(message));
                                    LoanResponse tempLR = (LoanResponse)serializer.Deserialize(memStream);
                                    JObject resultingMessage = JObject.FromObject(tempLR);

                                    Console.WriteLine("Received: {0}", resultingMessage.ToString());
                                    resultingMessage.Add("bankName", XMLBANK);
                                    var resultBody = Encoding.UTF8.GetBytes(resultingMessage.ToString());

                                    Console.WriteLine("Header was: " + Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"]) + "\n");
                                    resultChannel.BasicPublish(exchange: RESULTEXCHANGE, routingKey: RESULTROUTINGKEY, basicProperties: ea.BasicProperties, body: resultBody);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Message was: \n");
                                    Console.WriteLine(message + "\n");
                                    Console.WriteLine("ERROR: \n");
                                    Console.WriteLine(e.Message + "\n");

                                    Console.WriteLine("Header was: " + Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"]) + "\n");
                                }
                            }
                            else if (language.Equals(LANGUAGESTRING))
                            {
                                //JObject messageJObject = JObject.Parse(message);
                                string sssn = message.Split(',')[0].Split(':')[1];
                                string intrestrate = message.Split(',')[1].Split(':')[1];
                                Console.WriteLine("Received: ssn: {0} : intrestrate: {1}", sssn ,intrestrate );
                                JObject messageJObject = new JObject();
                                messageJObject.Add("ssn",sssn);
                                messageJObject.Add("interestRate",intrestrate);
                                messageJObject.Add("bankName", LAURBAERBANK);
                                var resultBody = Encoding.UTF8.GetBytes(messageJObject.ToString());

                                Console.WriteLine("Header was: " + Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"]) + "\n");
                                resultChannel.BasicPublish(exchange: RESULTEXCHANGE, routingKey: RESULTROUTINGKEY, basicProperties: ea.BasicProperties, body: resultBody);
                            }   
                            else
                            {
                                Console.WriteLine("Language-Header was not null, but not correct either. \nHeader was:" + Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"]));
                            }
                        }
                        else
                        {
                            Console.WriteLine("Language-Header was null");
                        }
                    };
            channel.BasicConsume(queue: QUEUE, noAck: true, consumer: consumer);

            Console.WriteLine("READY FOR ACTION");
            Console.ReadLine();
        }
    }
}