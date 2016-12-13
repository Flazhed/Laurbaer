//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Aggregator.Entity;
//using Aggregator.Routes;

//namespace Aggregator
//{
//    public class BankQuoteAggregate
//    {
//        //Antal beskeder forventet
//        //Best rate logik
//        //Liste med beskeder

//        private List<BankReply> bankReplies;
//        private int messageCount;
//        private BankReply bestReply;
//        private readonly int? expectedMessages;
//        private string routingKey = "";
//        private MessageRouter messageRouter;

//        public BankQuoteAggregate(int? expectedMessages, MessageRouter messageRouter)
//        {
//            this.messageRouter = messageRouter;
//            this.bankReplies = new List<BankReply>();
//            this.expectedMessages = expectedMessages;
//            this.messageCount = 0;
//        }

//        public void AddBankReply(BankReply bankReply)
//        {
//            messageCount++;
//            if (messageCount == expectedMessages)
//            {
//                bankReplies.Add(bankReply);
//                var bestRate = bankReplies.Aggregate((i1, i2) => i1.interestRate < i2.interestRate ? i1 : i2);
//                Console.WriteLine("Best rate: {0} with bank: {1}", bestRate.interestRate, bestRate.bankName);
//                messageRouter.SendToRecipientList(bestRate);
//            }
//            else
//            {
//                bankReplies.Add(bankReply);
//            }
//        }
//    }
//}