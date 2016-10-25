using System;
using System.IO;
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
        public void translate()
        {
            var RecivedFormat = RMQCon.readQueue();
            //do translation
            var translatedformat = this.TransformMessage(RecivedFormat);// do the translation here
            RMQCon.SendXMLToBankQueue(translatedformat);
        }

        private string TransformMessage(string recivedFormat)
        {
            XMLBank.LoanRequest bankFormatLoanRequest = new XMLBank.LoanRequest();
            LoanBroker.LoanRequest recivedFormatLoanRequest = ObjectFromXML<LoanBroker.LoanRequest>(recivedFormat);
            bankFormatLoanRequest.ssn = recivedFormatLoanRequest.ssn;
            bankFormatLoanRequest.creditScore = recivedFormatLoanRequest.creditScore;
            bankFormatLoanRequest.loanAmount = recivedFormatLoanRequest.loanAmount;
            bankFormatLoanRequest.loanDuration = recivedFormatLoanRequest.loanDuration;
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
    }

}