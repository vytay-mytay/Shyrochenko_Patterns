using Newtonsoft.Json;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class RegisterResponseModel
    {
        [JsonRequired]
        public string Email { get; set; }
    }
}