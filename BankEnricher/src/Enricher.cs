using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;

namespace BankEnricher.src
{
    public class Enricher
    {

        public static string Enrich(string message)
        {
            JObject messageJObject = JObject.Parse(message);
            float loanAmount = float.Parse(messageJObject.GetValue("loanAmount").ToString());
            float loanDuration = float.Parse(messageJObject.GetValue("loanDuration").ToString());
            int creditScore = int.Parse(messageJObject.GetValue("creditScore").ToString());

            string banksjson = new RuleFetcher().GetBanksJson(loanDuration, loanAmount, creditScore);
            JArray bankJarr = JArray.Parse(banksjson);
            messageJObject.Add("banks", bankJarr);
            Console.WriteLine("BanksJson: ", banksjson);

            return messageJObject.ToString();
        }





    }
}
