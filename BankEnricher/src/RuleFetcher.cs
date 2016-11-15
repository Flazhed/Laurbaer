using BankEnricher.BankRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankEnricher.src
{
    public class RuleFetcher
    {

        public string GetBanksJson(float loanDuration, float loanAmount, int creditScore)
        {
            RuleBaseWSClient bankRules = new RuleBaseWSClient();
            string banksJson = bankRules.rules(loanDuration, loanAmount, creditScore);
            return banksJson;
        }

    }
}
