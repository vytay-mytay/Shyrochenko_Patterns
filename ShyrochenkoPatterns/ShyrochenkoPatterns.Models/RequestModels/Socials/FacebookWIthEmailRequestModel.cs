using ShyrochenkoPatterns.Common.Attributes;
using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels.Socials
{
    public class FacebookWithEmailRequestModel : BridgeLoginRequestModel, BridgeRegisterRequestModel
    {
        [Required(ErrorMessage = "Token is empty")]
        public string Token { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessage = "Email address is not in valid format")]
        [StringLength(129, ErrorMessage = "Email address is not in valid format", MinimumLength = 3)]
        [CustomRegularExpression(ModelRegularExpression.REG_EMAIL, ErrorMessage = "Email address is not in valid format")]
        [CustomRegularExpression(ModelRegularExpression.REG_EMAIL_DOMAINS, ErrorMessage = "Email address is not in valid format")]
        public string Email { get; set; }
    }
}
