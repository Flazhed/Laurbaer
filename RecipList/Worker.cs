using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RecipList
{
    public class Worker
    {
        private readonly ConnectionFactory _factory;
        private readonly MessageRouter _messageRouter;
        private readonly BankService _bankService;

        public Worker(ConnectionFactory factory, MessageRouter messageRouter, BankService bankService)
        {
            _factory = factory;
            _messageRouter = messageRouter;
            _bankService = bankService;
        }

        public void CreateWorker()
        {
            var channel = CreateChannel();

            channel.ExchangeDeclare(exchange: Constants.ExchangeName, type: Constants.ExhangeType);

            //TODO: Define own.
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName, exchange: Constants.ExchangeName, routingKey: Constants.RoutingKey);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Msg:: " + message);
                // TODO: Take msg and send to serive, when format is in place.
                var eligbaleLenders = _bankService.GetEligibleBankQueues(0, 0, 0);
                _messageRouter.SendToRecipientList(null, eligbaleLenders);
            };

            channel.BasicConsume(queue: queueName,
                noAck: true,
                consumer: consumer);
        }


        private IModel CreateChannel()
        {
            a: var connection = _factory.CreateConnection();
            return connection.CreateModel();
        }
    }
}