using System.Xml.Serialization;

namespace Normalizer
{
    [XmlRoot(ElementName = "LoanRequest")]
    public class LoanRequest
    {
        [XmlElement(ElementName = "interestRate")]
        public string interestRate { get; set; }
        [XmlElement(ElementName = "ssn")]
        public string ssn { get; set; }
    }
}
