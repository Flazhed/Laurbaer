using System;


namespace TranslatorWebserviceBank.WebServiceBank
{
    //JsonBank format
    public class LoanRequest
    {
        public string ssn { get; set; }
        public int creditScore { get; set; }
        public float loanAmount { get; set; }
        public int loanDuration { get; set; }
    }
}
