using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels.Socials
{
    public class FacebookWithPhoneRequestModel : BridgeRegisterRequestModel, BridgeLoginRequestModel
    {
        [Required(ErrorMessage = "Token is empty")]
        public string Token { get; set; }

        [RegularExpression(ModelRegularExpression.REG_PHONE, ErrorMessage = "Phone number is not in valid format")]
        [StringLength(14, ErrorMessage = "Phone number is not in valid format", MinimumLength = 13)]
        public string PhoneNumber { get; set; }
    }
}
