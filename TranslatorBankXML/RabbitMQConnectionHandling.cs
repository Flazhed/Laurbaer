using System;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TranslatorBankXML
{
    class RabbitMQConnectionHandling
    {
        private ConnectionFactory factory;
        private static string _Host_Name = "datdb.cphbusiness.dk";
        private static string _Username = "student";
        private static string _Password = "cph";
        private static string _directExchangeName = "laurbaer_direct";
        private IConnection connection;
        private IModel channel;
        public RabbitMQConnectionHandling()
        {
            factory = new ConnectionFactory() { HostName = _Host_Name, UserName =_Username, Password=_Password };
        }
        public void OpenCon()
        {
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
        }
        public void CloseConn()
        {
            channel.Close();
            channel.Dispose();
            channel = null;
            connection.Close();
            connection.Dispose();
            connection = null;
        }
        public void StartReadQueue()
        {
            channel.ExchangeDeclare(exchange: _directExchangeName, type: "direct");
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: _directExchangeName, routingKey: "laurbaer_xml_translator");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += EventBasicConsumer_Recieved;

            channel.BasicConsume(queue: queueName,
                                 noAck: true,
                                 consumer: consumer);
        }
        private static void EventBasicConsumer_Recieved(object sender, BasicDeliverEventArgs e)
        {
            RabbitMQTranslator aTranslator = new RabbitMQTranslator();
            string messageRecieved = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine(" [x] Received {0}", messageRecieved);

            string[] translatedFormat = aTranslator.Translate(messageRecieved);
            SendXMLToBankQueue(translatedFormat[0], translatedFormat[1], e); 
            
            Console.WriteLine("exit? {yes/[no]}: ");
        }
        public static void SendXMLToBankQueue(string XMLBankFormat, string exchangeName, BasicDeliverEventArgs e)
        {
            var factory = new ConnectionFactory() { HostName = _Host_Name };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");

                string message = XMLBankFormat;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: e.BasicProperties, body: body);
                Console.WriteLine(" [x] Sent {0} with basicproperties : {1}", message, e.BasicProperties.ToString());
            }
        }
    }
}
