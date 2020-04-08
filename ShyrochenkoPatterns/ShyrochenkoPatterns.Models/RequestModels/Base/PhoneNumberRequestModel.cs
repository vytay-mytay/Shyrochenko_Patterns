using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels
{
    public class PhoneNumberRequestModel
    {
        [Required(ErrorMessage = "Phone number field is empty")]
        [RegularExpression(ModelRegularExpression.REG_PHONE, ErrorMessage = "Phone number is not in valid format")]
        [StringLength(14, ErrorMessage = "Phone number is not in valid format", MinimumLength = 13)]
        public string PhoneNumber { get; set; }
    }
}
