using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels
{
    public class TokenRequestModel
    {
        [JsonProperty("token")]
        [Required(ErrorMessage = "Is required")]
        public string Token { get; set; }
    }
}
