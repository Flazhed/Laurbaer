namespace RecipList
{
    public class Bank
    {
        private int loanAmount;
        private int creditscore;
        private int historLength;
        public string name { get; set; }
        public string queue { get; set; }

        public Bank(string name, int loanAmount, int creditscore, int historLength, string queue)
        {
            this.name = name;
            this.loanAmount = loanAmount;
            this.creditscore = creditscore;
            this.historLength = historLength;
            this.queue = queue;
        }


        public bool CanHandleLoanRequest(int CreditScore, int HistoryLength, int
            LoanAmount)
        {
            return LoanAmount >= loanAmount && CreditScore >= creditscore && HistoryLength >= historLength;
        }
    }
}