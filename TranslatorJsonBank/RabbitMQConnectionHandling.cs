using System;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TranslatorJsonBank
{
    class RabbitMQConnectionHandling
    {
        private ConnectionFactory factory;
        private static string _BankXMLQueue_Name = "BankJson";
        private static string _LoanRequestQueue_Name = "BlorQ";
        private static string _Host_Name = "datdb.cphbusiness.dk";
        private static string _Username = "student";
        private static string _Password = "cph";
        public RabbitMQConnectionHandling()
        {
            factory = new ConnectionFactory() { HostName = _Host_Name, UserName =_Username, Password=_Password };
        }
        public void StartReadQueue()
        {
            //string message = null;
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _LoanRequestQueue_Name,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    channel.QueueBind(queue: _LoanRequestQueue_Name, exchange: "BlorQex", routingKey: "BlorQ");
                    //var consumer = new QueueingBasicConsumer(channel);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += EventBasicConsumer_Recieved;
                        
                    channel.BasicConsume(queue: _LoanRequestQueue_Name,
                                         noAck: true,
                                         consumer: consumer);
                    //var eventArgs = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                    //message = Encoding.UTF8.GetString(eventArgs.Body);
                    //Console.WriteLine(message);

                    //consumer.Received += (model, ea) =>
                    //{
                    //    var body = ea.Body;
                    //    var internalmessage = Encoding.UTF8.GetString(body);

                    //    Console.WriteLine(" [x] Received {0}", internalmessage);

                    //    Console.WriteLine(" Press [enter] to exit.");
                    //    Console.ReadLine();
                    //};

                    //Console.WriteLine(" [x] Received {0}", message);


                }
            }
        }
        private static void EventBasicConsumer_Recieved(object sender, BasicDeliverEventArgs e)
        {
            RabbitMQTranslator aTranslator = new RabbitMQTranslator();
            string messageRecieved = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine(" [x] Received {0}", messageRecieved);
           
            string translatedFormat = aTranslator.Translate(messageRecieved);
            SendJsonToBankQueue(translatedFormat,e);

            Console.WriteLine(" [x] send {0}", translatedFormat);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();

        }

        public static void SendJsonToBankQueue(string JsonBankFormat, BasicDeliverEventArgs e)
        {
            var factory = new ConnectionFactory() { HostName = _Host_Name };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _BankXMLQueue_Name, durable: false, exclusive: false, autoDelete: false, arguments: null);

                string message = JsonBankFormat;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "", routingKey: _BankXMLQueue_Name, basicProperties: e.BasicProperties, body: body);
                Console.WriteLine(" [x] Sent {0} with basicproperties : {1}", message,e.BasicProperties.ToString());
            }
        }
    }
}
