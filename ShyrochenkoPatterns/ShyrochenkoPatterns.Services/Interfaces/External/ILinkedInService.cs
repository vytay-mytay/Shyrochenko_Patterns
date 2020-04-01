using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.RequestModels.Socials;
using ShyrochenkoPatterns.Models.ResponseModels;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.External
{
    public interface ILinkedInService
    {
        Task<LIProfileResponseModel> GetProfile(string token, string redirectUri, bool emailRequest = false);

        Task<LoginResponseModel> ProcessRequest(LinkedInWithEmailRequestModel model);

        Task<LoginResponseModel> ProcessRequest(LinkedInWithPhoneRequestModel model);

        Task<LoginResponseModel> ConfrimRegistration(ConfirmPhoneRequestModel model);
    }
}
