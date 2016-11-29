using System;


namespace TranslatorBankXML.LoanBroker
{
    ////XMLBank format
    //public class LoanRequest
    //{
    //    public long ssn { get; set; }
    //    public int creditScore { get; set; }
    //    public float loanAmount { get; set; }
    //    public DateTime loanDuration { get; set; }
    //}

    public class Bank
    {
        public string bankName { get; set; }
        public string translatorRoutingKey { get; set; }
        public string fanoutName { get; set; }
    }

    public class LoanRequest
    {
        public string ssn { get; set; }
        public float loanAmount { get; set; }
        public int loanDuration { get; set; }
        public int creditScore { get; set; }
        public Bank bank { get; set; }
    }
}
