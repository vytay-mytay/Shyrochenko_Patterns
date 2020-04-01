using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.Enums;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces
{
    public interface ISMSService
    {
        Task<bool> SendVerificationCodeAsync(ApplicationUser user, string phoneNumber, VerificationCodeType type, string data = null);

        Task<bool> SendVerificationCodeAsync(string phoneNumber, VerificationCodeType type, string data = null);
    }
}
