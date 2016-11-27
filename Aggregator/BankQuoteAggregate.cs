using System.Collections.Generic;
using System.Linq;
using Aggregator.Entity;
using Aggregator.Routes;

namespace Aggregator
{
    public class BankQuoteAggregate
    {
        //Antal beskeder forventet
        //Best rate logik
        //Liste med beskeder

        private List<BankReply> bankReplies;
        private int messageCount;
        private BankReply bestReply;
        private readonly int? expectedMessages;
        private string routingKey = "";
        private MessageRouter messageRouter;

        public BankQuoteAggregate(int? expectedMessages, MessageRouter messageRouter)
        {
            this.messageRouter = messageRouter;
            this.bankReplies = new List<BankReply>();
            this.expectedMessages = expectedMessages;
            this.messageCount = 0;
        }

        public void AddBankReply(BankReply bankReply)
        {
            messageCount++;
            if (messageCount == expectedMessages)
            {
                var bestRate = bankReplies.Aggregate((i1, i2) => i1.InterestRate > i2.InterestRate ? i1 : i2);
                messageRouter.SendToRecipientList(bestRate);
            }
            else
            {
                bankReplies.Add(bankReply);
            }
        }
    }
}