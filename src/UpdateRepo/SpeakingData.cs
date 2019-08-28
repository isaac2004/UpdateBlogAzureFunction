using Newtonsoft.Json;

namespace UpdateRepo
{
    public class Event
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "eventName")]
        public string EventName { get; set; }
        [JsonProperty(PropertyName = "location")]
        public string Location { get; set; }
        [JsonProperty(PropertyName = "talks")]
        public string Talks { get; set; }
        [JsonProperty(PropertyName = "startDate")]
        public string StartDate { get; set; }
        [JsonProperty(PropertyName = "endDate")]
        public string EndDate { get; set; }
        [JsonProperty(PropertyName = "done")]
        public string Done { get; set; }
    }
}
