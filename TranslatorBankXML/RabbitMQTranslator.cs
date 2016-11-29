using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TranslatorBankXML
{
    public class RabbitMQTranslator
    {
        private RabbitMQConnectionHandling RMQCon;
    
        public RabbitMQTranslator()
        {
            RMQCon = new RabbitMQConnectionHandling();
        }
        public string Translate(string RecivedFormat)
        {
            var translatedformat = this.TransformMessage(RecivedFormat);// do the translation here
            //RMQCon.SendXMLToBankQueue(translatedformat);
            return translatedformat;
        }

        private string TransformMessage(string recivedFormat)
        {
            XMLBank.LoanRequest bankFormatLoanRequest = new XMLBank.LoanRequest();
            LoanBroker.LoanRequest recivedFormatLoanRequest = ObjectFromJson<LoanBroker.LoanRequest>(recivedFormat);
            bankFormatLoanRequest.ssn = recivedFormatLoanRequest.ssn;
            bankFormatLoanRequest.creditScore = recivedFormatLoanRequest.creditScore;
            bankFormatLoanRequest.loanAmount = recivedFormatLoanRequest.loanAmount;
            bankFormatLoanRequest.loanDuration = new DateTime(1970,1,1).AddMonths(recivedFormatLoanRequest.loanDuration);
            return GetXMLFromObject(bankFormatLoanRequest);
        }

        private T ObjectFromXML<T>(string xml)
        {
            StringReader strReader = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            T obj = default(T);
            try
            {
                strReader = new StringReader(xml);
                serializer = new XmlSerializer(typeof(T));
                xmlReader = new XmlTextReader(strReader);
                obj = (T)serializer.Deserialize(xmlReader);
            }
            catch (Exception exp)
            {
                //Handle Exception Code
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
                if (strReader != null)
                {
                    strReader.Close();
                }
            }
            return obj;
        }

        private static string GetXMLFromObject(object o)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(o.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, o);
            }
            catch (Exception ex)
            {
                //Handle Exception Code
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
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