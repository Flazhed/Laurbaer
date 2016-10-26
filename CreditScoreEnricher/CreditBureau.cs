using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreditScoreEnricher
{
    public class CreditBureau
    {

        public static int GetCreditScore(string ssn)
        {
            Random rand = new Random();
            return rand.Next(0,800);
        }

    }
}
