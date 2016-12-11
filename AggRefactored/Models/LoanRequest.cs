//using System.Collections.Generic;
//using Aggregator.Entity;
//using Newtonsoft.Json;

//namespace Aggregator.Entity
//{
//    public class LoanRequest
//    {
//        //Can be left blank, and will not get serialized.
//        public string ssn { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public float? loanAmount { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public float? loanDuration { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public int? creditScore { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public List<Bank> banks { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public Bank bank { get; set; }

//        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
//        public int? count { get; set; }
//    }
//}