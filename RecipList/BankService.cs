using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;

namespace RecipList
{
    public class BankService
    {
        private Bank[] _banks;

        public BankService(Bank[] banks)
        {
            _banks = banks;
        }

        public List<Bank> GetEligibleBankQueues(int CreditScore, int HistoryLength,
            int LoanAmount)
        {
            List<Bank> lenders = new List<Bank>();
            foreach (var bankConnection in _banks)
            {
                if (bankConnection.CanHandleLoanRequest(CreditScore, HistoryLength,
                    LoanAmount))
                    lenders.Add(bankConnection);
            }

            return lenders;
        }

    }
}