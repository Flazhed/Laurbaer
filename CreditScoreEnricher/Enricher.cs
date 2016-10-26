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



        public static string Enrich(string message)
        {
            //extract SSN
            string ssn = "lel";
            string creditscore = "Credit score: ";
            return creditscore +  CreditBureau.GetCreditScore(ssn);
        }





    }
}
