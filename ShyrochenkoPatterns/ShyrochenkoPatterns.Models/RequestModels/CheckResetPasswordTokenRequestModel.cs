using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels
{
    public class CheckResetPasswordTokenRequestModel : EmailRequestModel
    {
        [Required(ErrorMessage = "Token is empty")]
        public string Token { get; set; }
    }
}
