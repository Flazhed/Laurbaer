using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;

namespace SoapWebService
{
    /// <summary>
    /// Summary description for LaurbaerService
    /// </summary>
    [WebService(Namespace = "laurbaer_namespace")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class LaurbaerService : System.Web.Services.WebService
    {
        private Sender sender;
        private Worker worker;
        private const string QueueName = "laurbaer_soap_response";

        [WebMethod]
        public string SoapLoanRequest(string ssn, float loanAmount, float loanDuration)
        {
            var msg = "Error";

            LoanRequest loanRequest = new LoanRequest {ssn = ssn, loanAmount = loanAmount, loanDuration = loanDuration};
            var corrId = Guid.NewGuid().ToString();

            worker = new Worker(QueueName);
            sender = new Sender(QueueName);
            var sended = sender.Send(loanRequest, corrId);

            if (sended)
            {
                msg = worker.Consume();
            }


            return msg;
        }
    }
}