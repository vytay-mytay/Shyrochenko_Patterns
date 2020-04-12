using Newtonsoft.Json;
using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels
{
    public class AdminLoginRequestModel : EmailRequestModel, BridgeLoginRequestModel
    {
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [JsonProperty("accessTokenLifetime")]
        public int? AccessTokenLifetime { get; set; }
    }
}
