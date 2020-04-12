using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.Bridge.Abstraction
{
    public interface IBridgeAbstraction
    {
        Task<BridgeLoginResponseModel> Login(BridgeLoginRequestModel model);

        Task<BridgeRegisterResponseModel> Register(BridgeRegisterRequestModel model);
    }
}
