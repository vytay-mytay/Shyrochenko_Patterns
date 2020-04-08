using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.RequestModels.Socials;
using ShyrochenkoPatterns.Models.ResponseModels;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.External
{
    public interface IFacebookService
    {
        Task<FBProfileResponseModel> GetProfile(string token);

        Task<LoginResponseModel> ProcessRequest(FacebookWithPhoneRequestModel model);

        Task<LoginResponseModel> ProcessRequest(FacebookWithEmailRequestModel model);

        Task<LoginResponseModel> ConfirmFacebookRegistration(ConfirmPhoneRequestModel model);
    }
}
