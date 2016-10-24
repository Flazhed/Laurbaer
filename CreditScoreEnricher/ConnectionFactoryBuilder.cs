using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditScoreEnricher
{
    public class ConnectionFactoryBuilder
    {

        public ConnectionFactory CreateConnectionFactory()
        {
            var factory = new ConnectionFactory() { HostName = "datdb.cphbusiness.dk", UserName = "student", Password = "cph" };
            return factory;
        }
    }
}
