using Microsoft.AspNetCore.Identity;
using ShyrochenkoPatterns.Common.Extensions;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using System.Linq;

namespace ShyrochenkoPatterns.Helpers
{
    public static class ApplicationUserManagerExtensions
    {
        public static ApplicationUser FindByPhone(this UserManager<ApplicationUser> userManager, string phoneNumber)
        {
            userManager.ThrowsWhenNull(nameof(userManager));
            phoneNumber.ThrowsWhenNullOrEmpty(nameof(phoneNumber));

            return userManager.Users.FirstOrDefault(user => user.PhoneNumber.Equals(phoneNumber));
        }

        public static bool IsEmailAlreadyExists(this UserManager<ApplicationUser> userManager, string email)
        {
            userManager.ThrowsWhenNull(nameof(userManager));
            email.ThrowsWhenNullOrEmpty(nameof(email));

            return userManager.Users.Any(user => user.EmailConfirmed && user.Email.ToLower().Equals(email.ToLower()));
        }

        public static bool IsPhoneAlreadyExists(this UserManager<ApplicationUser> userManager, string phoneNumber)
        {
            userManager.ThrowsWhenNull(nameof(userManager));
            phoneNumber.ThrowsWhenNullOrEmpty(nameof(phoneNumber));

            return userManager.Users.Any(user => user.PhoneNumberConfirmed && user.PhoneNumber.Equals(phoneNumber));
        }
    }

}
