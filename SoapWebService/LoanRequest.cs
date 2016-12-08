namespace SoapWebService
{
    public class LoanRequest
    {
        public string ssn { get; set; }
        public float loanAmount { get; set; }
        public float loanDuration { get; set; }
    }
}