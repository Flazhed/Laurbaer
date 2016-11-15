using CreditScoreEnricher.CreditScoreClient;
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
            CreditScoreServiceClient creditScoreClient = new CreditScoreServiceClient();
            int creditScore;
            try
            {
                creditScore = creditScoreClient.creditScore(ssn);
            }
            catch(Exception e)
            {
                //we catching generic exception, team!
                creditScore = 0;
            }
            return creditScore;
        }

    }
}
