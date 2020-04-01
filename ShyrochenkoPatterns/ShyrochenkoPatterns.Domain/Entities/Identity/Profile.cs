using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShyrochenkoPatterns.Domain.Entities.Identity
{
    public class Profile : IEntity<int>
    {
        #region Properties

        public int Id { get; set; }

        public int UserId { get; set; }

        [MaxLength(30)]
        public string FirstName { get; set; }

        [MaxLength(30)]
        public string LastName { get; set; }

        public int? AvatarId { get; set; }

        public string BraintreeCustomerId { get; set; }

        public string StripeCustomerId { get; set; }

        #endregion

        #region Navigation properties

        [InverseProperty("Profile")]
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("AvatarId")]
        public virtual Image Avatar { get; set; }

        #endregion

        #region Additional Properties

        [NotMapped]
        public string FullName
        {
            get
            {
                if (!String.IsNullOrEmpty(FirstName) && !String.IsNullOrEmpty(LastName))
                    return $"{FirstName} {LastName}";
                else
                    return String.Empty;
            }
        }

        #endregion
    }
}
