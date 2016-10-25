using System;


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
