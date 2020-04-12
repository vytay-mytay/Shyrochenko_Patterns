using Newtonsoft.Json;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class RegisterUsingPhoneResponseModel : BridgeRegisterResponseModel
    {
        [JsonRequired]
        public string Phone { get; set; }

        [JsonRequired]
        public bool SMSSent { get; set; }
    }
}
