using Newtonsoft.Json;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class RegisterResponseModel : BridgeRegisterResponseModel
    {
        [JsonRequired]
        public string Email { get; set; }
    }
}