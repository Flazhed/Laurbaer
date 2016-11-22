using Newtonsoft.Json;

namespace RecipList
{
    public class JsonConverter
    {
        public string ssn { get; set; }
        public float loanAmount { get; set; }
        public float loanDuration { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Reference { get; set; }


        public JsonConverter ConvertFromJson(string msg)
        {
            return null;
        }
    }
}