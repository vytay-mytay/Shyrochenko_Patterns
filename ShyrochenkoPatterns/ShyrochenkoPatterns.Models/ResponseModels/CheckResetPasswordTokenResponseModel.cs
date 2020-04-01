using Newtonsoft.Json;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class CheckResetPasswordTokenResponseModel
    {
        [JsonRequired]
        public bool IsValid { get; set; }
    }
}