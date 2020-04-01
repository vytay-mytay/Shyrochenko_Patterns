using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.RequestModels.Socials;
using ShyrochenkoPatterns.Models.ResponseModels;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.External
{
    public interface IGoogleService
    {
        Task<GProfileResponseModel> GetProfile(string token);

        Task<LoginResponseModel> ProcessRequest(GoogleWithPhoneRequestModel model);

        Task<LoginResponseModel> ProcessRequest(GoogleWithEmailRequestModel model);

        Task<LoginResponseModel> ConfrimRegistration(ConfirmPhoneRequestModel model);
    }
}
