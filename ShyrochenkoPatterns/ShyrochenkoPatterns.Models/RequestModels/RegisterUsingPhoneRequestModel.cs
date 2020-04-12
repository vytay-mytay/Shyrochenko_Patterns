using Newtonsoft.Json;
using ShyrochenkoPatterns.Common.Attributes;
using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels
{
    public class RegisterUsingPhoneRequestModel : PhoneNumberRequestModel, BridgeRegisterRequestModel
    {
        [CustomRegularExpression(ModelRegularExpression.REG_ONE_LATER_DIGIT, ErrorMessage = "Password should contain at least one letter and one digit")]
        [CustomRegularExpression(ModelRegularExpression.REG_NOT_CONTAIN_SPACES_ONLY, ErrorMessage = "Password can’t contain spaces only")]
        [Required(ErrorMessage = "Password field is empty")]
        [StringLength(50, ErrorMessage = "Password should be from 6 to 50 characters", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password field is empty")]
        [Compare("Password", ErrorMessage = "Confirm Password isn’t the same as Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
