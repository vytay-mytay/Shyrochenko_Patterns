using Newtonsoft.Json;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class FBProfileResponseModel
    {
        public string Id { get; set; }

        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }
    }
}
