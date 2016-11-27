using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aggregator.Routes;
using Aggregator.Utils;
using RabbitMQ.Client;

namespace Aggregator
{
    class Program
    {
        private readonly ConnectionFactory _factory;
        private readonly MessageRouter _messageRouter;
        private readonly BankGateway _bankGateway;

        public Program()
        {
            Console.WriteLine("Init Aggregator");
            _factory = new ConnectionFactory
            {
                HostName = Constants.Host,
                Port = Constants.Port,
                UserName = Constants.UserName,
                Password = Constants.Password
            };
            _messageRouter = new MessageRouter(_factory);
            _bankGateway = new BankGateway(_factory, _messageRouter);
        }

        private void Run()
        {
            _bankGateway.CreateConsumer();
            Console.WriteLine("Ready to receive");
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }
    }
}