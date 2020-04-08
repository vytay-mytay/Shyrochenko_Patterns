using Newtonsoft.Json;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class IdResponseModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
