using System;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TranslatorJsonBank
{
    class RabbitMQConnectionHandling
    {
        private ConnectionFactory factory;
        private IConnection connection;
        private IModel channel;
        public RabbitMQConnectionHandling()
        {
            factory = new ConnectionFactory() { HostName = StaticHardcodedVariables.host_Name, UserName = StaticHardcodedVariables.username, Password = StaticHardcodedVariables.password };
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
        public void ReadQueue()
        {
            channel.ExchangeDeclare(exchange: StaticHardcodedVariables.directExchangeName, type: "direct");
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: StaticHardcodedVariables.directExchangeName, routingKey: StaticHardcodedVariables.routingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += EventBasicConsumer_Recieved;

            channel.BasicConsume(queue: queueName,
                                 noAck: true,
                                 consumer: consumer);


        }
        private static void EventBasicConsumer_Recieved(object sender, BasicDeliverEventArgs e)
        {
            IRabbitMQTranslator aTranslator = TranslatorFactory.GetTranslator();
            string messageRecieved = Encoding.UTF8.GetString(e.Body);
            Console.WriteLine(" [x] Received {0}", messageRecieved);

            string[] translatedFormat = aTranslator.Translate(messageRecieved);
            SendToBankQueue(translatedFormat[0], translatedFormat[1], e);

            Console.WriteLine("exit? {yes/[no]}: ");

        }

        public static void SendToBankQueue(string BankFormat, string exchangeName, BasicDeliverEventArgs e)
        {
            var factory = new ConnectionFactory() { HostName = StaticHardcodedVariables.host_Name };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchangeName, type: "fanout");
              
                string message = BankFormat;
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchangeName, routingKey: "", basicProperties: e.BasicProperties, body: body);
                Console.WriteLine(" [x] Sent {0} with basicproperties : {1}", message, e.BasicProperties.ToString());
            }
        }
    }
}
