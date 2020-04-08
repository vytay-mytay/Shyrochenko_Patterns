using Newtonsoft.Json;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class RegisterUsingPhoneResponseModel
    {
        [JsonRequired]
        public string Phone { get; set; }

        [JsonRequired]
        public bool SMSSent { get; set; }
    }
}
