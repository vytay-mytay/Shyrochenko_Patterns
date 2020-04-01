using System.ComponentModel.DataAnnotations;

namespace ShyrochenkoPatterns.Models.RequestModels
{
    public class UserProfileRequestModel
    {
        [Required(ErrorMessage = "FirstName is required")]
        [StringLength(30, ErrorMessage = "FirstName must be from 1 to 30 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_MUST_NOT_CONTAIN_SPACES, ErrorMessage = "FirstName must not contain spaces")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        [StringLength(30, ErrorMessage = "LastName must be from 1 to 30 symbols", MinimumLength = 1)]
        [RegularExpression(ModelRegularExpression.REG_MUST_NOT_CONTAIN_SPACES, ErrorMessage = "LastName must not contain spaces")]
        public string LastName { get; set; }

        public int? ImageId { get; set; }
    }
}
