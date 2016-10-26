using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditScoreEnricher
{
    public class Runner
    {
        static void Main(string[] args)
        {
            ConnectionFactory factory = new ConnectionFactoryBuilder().CreateConnectionFactory();
            new Reciever().Recieve(factory);
        }

    }
}
