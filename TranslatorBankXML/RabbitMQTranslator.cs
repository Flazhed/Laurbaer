using System;
using System.Text;

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
            var translatedformat = RecivedFormat;// do the translation here
            RMQCon.SendXMLToBankQueue(translatedformat);
        }
       
       
    }

}