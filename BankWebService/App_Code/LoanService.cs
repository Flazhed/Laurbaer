using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;

/// <summary>
/// Summary description for LoanService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class LoanService : System.Web.Services.WebService
{

    public LoanService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    //[WebMethod]
    //public bool LoanRequestSendToReplyTo(string ssn,int creditScore,double loanAmount,int months,string replyTo,int correlationID)
    //{
    //    string result = this.LoanRequest(ssn, creditScore, loanAmount, months);

    //    var factory = new ConnectionFactory() { HostName = BankWebService.App_Code.StaticHardcodedVariables.host_Name };
    //    using (var connection = factory.CreateConnection())
    //    using (var channel = connection.CreateModel())
    //    {

    //        channel.ExchangeDeclare(exchange: replyTo, type: "direct");

    //        var props = channel.CreateBasicProperties();
    //        props.CorrelationId = correlationID.ToString();
    //        props.Headers.Add("language", "string");
    //        channel.BasicPublish(exchange: replyTo, routingKey: "", basicProperties: props, body: Encoding.UTF8.GetBytes(result));
    //        //Console.WriteLine(" [x] Sent {0} with basicproperties : {1}", e.Body, e.BasicProperties.ToString());
    //    }
    //    return false;
    //}
    //[WebMethod]
    //public void LoanRequest1(string ssn, int creditScore, double loanAmount, int months,IBasicProperties e)
    //{
    //    var factory = new ConnectionFactory() { HostName = BankWebService.App_Code.StaticHardcodedVariables.host_Name };
    //    using (var connection = factory.CreateConnection())
    //    using (var channel = connection.CreateModel())
    //    {
            
    //        channel.ExchangeDeclare(exchange: e.ReplyTo, type: "direct");

    //        string body = this.LoanRequest(ssn, creditScore, loanAmount, months);
            
    //        channel.BasicPublish(exchange: e.ReplyTo, routingKey: "", basicProperties: e, body: Encoding.UTF8.GetBytes(body)    );
    //        //Console.WriteLine(" [x] Sent {0} with basicproperties : {1}", e.Body, e.BasicProperties.ToString());
    //    }
    //}
    [WebMethod]
    public string LoanRequest(string ssn, int creditScore, double loanAmount, int months)
    {

        return "ssn: "+ssn+", intrestrate: 4.5" ;
    }

}
