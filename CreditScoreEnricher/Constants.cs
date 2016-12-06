using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditScoreEnricher
{
    public class Constants
    {
        public static string DirectExchanceType = "direct";
        public static string DirectExchanceName = "laurbaer_direct";
        public static string DirectRoutingKeyEnriched = "credit_score_enriched";
        public static string DirectRoutingKeyReciever = "credit_score_not_enriched";
        public static string Host = "datdb.cphbusiness.dk";
        public static int Port = 5672;
        public static string UserName = "student";
        public static string Password = "cph";

    }
}
