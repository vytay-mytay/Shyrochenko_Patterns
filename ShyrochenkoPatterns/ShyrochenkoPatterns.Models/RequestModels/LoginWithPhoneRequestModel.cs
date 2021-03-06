using Newtonsoft.Json;
using ShyrochenkoPatterns.Common.Attributes;
using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels
{
    public class LoginWithPhoneRequestModel : PhoneNumberRequestModel, BridgeLoginRequestModel
    {
        [StringLength(50, ErrorMessage = "Password should be from 6 to 50 characters", MinimumLength = 6)]
        [CustomRegularExpression(ModelRegularExpression.REG_ONE_LATER_DIGIT, ErrorMessage = "Password should contain at least one letter and one digit")]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES_ONLY, ErrorMessage = "Password can’t contain spaces only")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [JsonProperty("accessTokenLifetime")]
        public int? AccessTokenLifetime { get; set; }
    }
}
