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
        private readonly ConnectionFactory _factory;
        private readonly MessageRouter _messageRouter;
        private readonly Worker _worker;

        public Program()
        {
            Console.WriteLine("Init Recipient list");
            _factory = new ConnectionFactory
            {
                HostName = Constants.Host,
                Port = Constants.Port,
                UserName = Constants.UserName,
                Password = Constants.Password
            };
            _messageRouter = new MessageRouter(_factory);
            _worker = new Worker(_factory, _messageRouter);
        }

        private void Run()
        {
            _worker.CreateConsumer();
            Console.WriteLine("Ready to receive");
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }
    }
}