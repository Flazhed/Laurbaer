using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RecipList
{
    public class Program
    {
        private Bank[] banks =
        {
            new Bank("DanskeBank", 10, 10, 10, "qName"),
            new Bank("Nordea", 10, 10, 10, "qName"),
            new Bank("JyskeBank", 10, 10, 10, "qName"),
        };

        private readonly ConnectionFactory _factory;
        private readonly BankService _bankService;
        private readonly MessageRouter _messageRouter;
        private readonly Worker _worker;

        public Program()
        {
            _factory = new ConnectionFactory
            {
                HostName = Constants.Host,
                Port = Constants.Port,
                UserName = Constants.UserName,
                Password = Constants.Password
            };
            _messageRouter = new MessageRouter(_factory);
            _bankService = new BankService(banks);
            _worker = new Worker(_factory, _messageRouter, _bankService);
        }

        private void Run()
        {
            _worker.CreateWorker();
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }
    }
}