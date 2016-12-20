using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;
using static Normalizer.Constants;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;
using System.IO;

namespace Normalizer
{
    class Consumer
    {
        public static EventingBasicConsumer CreateConsumer(IModel channel, IModel resultChannel)
        {
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
                        {
                            if (Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"]) != null)
                            {
                                var language = (Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"])).ToLower();

                                var body = ea.Body;
                                string message = Encoding.UTF8.GetString(body);
                                JObject messageJObject = new JObject();

                                if (language.Equals(LANGUAGEJSON))
                                {
                                    messageJObject = JObject.Parse(message);
                                    Console.WriteLine("Received: {0}", messageJObject.ToString());
                                    messageJObject.Add("bankName", JSONBANK);
                                }
                                else if (language.Equals(LANGUAGEXML))
                                {
                                    try
                                    {
                                        XmlSerializer serializer = new XmlSerializer(typeof(LoanResponse));
                                        MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(message));
                                        LoanResponse tempLR = (LoanResponse)serializer.Deserialize(memStream);
                                        messageJObject = JObject.FromObject(tempLR);

                                        Console.WriteLine("Received: {0}", messageJObject.ToString());
                                        messageJObject.Add("bankName", XMLBANK);
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
                                    string ssn = message.Split(',')[0].Split(':')[1];
                                    string interestRate = message.Split(',')[1].Split(':')[1];
                                    Console.WriteLine("Received: ssn: {0} : interestRate: {1}", ssn, interestRate);
                                    messageJObject.Add("ssn", ssn);
                                    messageJObject.Add("interestRate", interestRate);
                                    messageJObject.Add("bankName", LAURBAERBANK);
                                }
                                else
                                {
                                    Console.WriteLine("Language-Header was not null, but not correct either. \nHeader was:" + Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"]));
                                }
                                var resultBody = Encoding.UTF8.GetBytes(messageJObject.ToString());

                                Console.WriteLine("Header was: " + Encoding.UTF8.GetString((Byte[])ea.BasicProperties.Headers["language"]) + "\n");
                                resultChannel.BasicPublish(exchange: RESULTEXCHANGE, routingKey: RESULTROUTINGKEY, basicProperties: ea.BasicProperties, body: resultBody);
                            }
                            else
                            {
                                Console.WriteLine("Language-Header was null");
                            }
                        };
            return consumer;
        }
    }
}
