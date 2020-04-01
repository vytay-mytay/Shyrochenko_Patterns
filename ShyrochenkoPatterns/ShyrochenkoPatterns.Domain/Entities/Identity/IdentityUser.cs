using Microsoft.AspNetCore.Identity;
using ShyrochenkoPatterns.Common.Extensions;
using ShyrochenkoPatterns.Domain.Entities.Chat;
using ShyrochenkoPatterns.Domain.Entities.Payment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShyrochenkoPatterns.Domain.Entities.Identity
{
    public partial class ApplicationUser : IdentityUser<int>, IEntity
    {
        #region Properties

        [Key]
        public override int Id { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        [DataType("DateTime")]
        public DateTime RegistratedAt { get; set; }

        [DataType("DateTime")]
        public DateTime? DeletedAt { get; set; }

        [DataType("DateTime")]
        public DateTime? LastVisitAt { get; set; }

        public string FacebookId { get; set; }

        public string GoogleId { get; set; }

        public string LinkedInId { get; set; }

        /// <summary>
        /// Difference between server and user timezones in hours 
        /// </summary>
        public double TimeZoneOffset { get; set; }

        #endregion

        #region Navigation Properties

        [InverseProperty("User")]
        public virtual Profile Profile { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserToken> Tokens { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<VerificationToken> VerificationTokens { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserDevice> Devices { get; set; }

        [InverseProperty("User")]
        public ICollection<ChatUser> Chats { get; set; }

        [InverseProperty("Creator")]
        public virtual ICollection<Message> Messages { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }

        [InverseProperty("User")]
        public ICollection<StripeSubscription> StripeSubscriptions { get; set; }

        #endregion

        #region Additional Properties

        [NotMapped]
        public DateTime ClientTime
        {
            get
            {
                return DateTime.UtcNow.AddHours(TimeZoneOffset);
            }
        }

        #endregion

        #region Ctors

        public ApplicationUser()
        {
            Tokens = Tokens.Empty();
            UserRoles = UserRoles.Empty();
            Devices = Devices.Empty();
            VerificationTokens = VerificationTokens.Empty();
        }

        #endregion
    }
}