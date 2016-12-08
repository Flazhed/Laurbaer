using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TranslatorWebserviceBank
{
    public class RabbitMQTranslator:IRabbitMQTranslator
    {
        //private RabbitMQConnectionHandling RMQCon;

        public RabbitMQTranslator()
        {
           // RMQCon = new RabbitMQConnectionHandling();
        }
        public string[] Translate(string RecivedFormat)
        {
            //var RecivedFormat = RMQCon.readQueue();
            //do translation
            var translatedformat = this.TransformMessage(RecivedFormat);// do the translation here
            //RMQCon.SendXMLToBankQueue(translatedformat);
            return translatedformat;
        }

        private string[] TransformMessage(string recivedFormat)
        {
            WebServiceBank.LoanRequest bankFormatLoanRequest = new WebServiceBank.LoanRequest();
            LoanBroker.LoanRequest recivedFormatLoanRequest = ObjectFromJson<LoanBroker.LoanRequest>(recivedFormat);
            bankFormatLoanRequest.ssn = recivedFormatLoanRequest.ssn.Replace("-","");
            bankFormatLoanRequest.creditScore = recivedFormatLoanRequest.creditScore;
            bankFormatLoanRequest.loanAmount = recivedFormatLoanRequest.loanAmount;
            bankFormatLoanRequest.loanDuration = recivedFormatLoanRequest.loanDuration;
            
            return new string[]{ recivedFormatLoanRequest.ssn.Replace("-", "")+": " + recivedFormatLoanRequest.creditScore + ": " + recivedFormatLoanRequest.loanAmount + ": " + recivedFormatLoanRequest.loanDuration, recivedFormatLoanRequest.bank.fanoutName };
        }
        private string GetJsonFromObject<T>(T o)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            var output = string.Empty;

            using (var ms = new MemoryStream())
            {
                jsonSerializer.WriteObject(ms, o);
                output = Encoding.UTF8.GetString(ms.ToArray());
            }
            return output;
        }
        private T ObjectFromJson<T>(string message) where T : class
        {
            object objResponse;
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(message)))
            {
                objResponse = jsonSerializer.ReadObject(ms);

            }
            return objResponse as T;
        }
    }


}