using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;

namespace CreditScoreEnricher
{
    public class Enricher
    {



        public static string Enrich(string message)
        {
            JObject messageJObject = JObject.Parse(message);
            string ssn = messageJObject.GetValue("ssn").ToString();
            messageJObject.Add("creditScore", CreditBureau.GetCreditScore(ssn));

            //Slyngel Deluxe, ryk til ny enricher
            JArray bankJArray = new JArray();
            JObject bank1 = new JObject();
            bank1.Add("bankName", "Bank1");
            bank1.Add("translatorRoutingKey", "Morten plz");
            bankJArray.Add(bank1);

            messageJObject.Add("banks", bankJArray);
            //

            return messageJObject.ToString();
        }





    }
}
