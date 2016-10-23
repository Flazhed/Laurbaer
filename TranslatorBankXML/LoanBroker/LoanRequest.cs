using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorBankXML.LoanBroker
{
    //XMLBank format
    public class LoanRequest
    {
        public long ssn { get; set; }
        public int creditScore { get; set; }
        public float loanAmount { get; set; }
        public DateTime loanDuration { get; set; }
    }
}
