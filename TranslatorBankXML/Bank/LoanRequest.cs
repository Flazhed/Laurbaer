using System;


namespace TranslatorBankXML.XMLBank
{
    //XMLBank format
    public class LoanRequest
    {
        public string ssn { get; set; }
        public int creditScore { get; set; }
        public float loanAmount { get; set; }
        public string loanDuration { get; set; }
    }
}
