using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslatorWebserviceBank
{
    public class TranslatorFactory
    {
        public static IRabbitMQTranslator GetTranslator()
        {
            return new RabbitMQTranslator();
        }
    }
   
}
