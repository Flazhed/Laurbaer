using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CreditScoreEnricher
{
    public class Enricher
    {
       public Enricher()
        {
            ConnectionFactory factory = new ConnectionFactoryBuilder().CreateConnectionFactory();
            //new Sender().Send(factory);
            new Reciever().Recieve(factory);
        }



        private void Enrich()
        {

        }





    }
}
