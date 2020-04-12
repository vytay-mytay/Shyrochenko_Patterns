using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Abstraction;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Implementation;
using ShyrochenkoPatterns.Services.Services.Bridge.Implementation;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Abstraction.Bridge
{
    public class BridgeUserEmail : IBridgeAbstraction
    {
        private BridgeUserEmailImplementation _implementation;

        public BridgeUserEmail(IBridgeImplementation implementation)
        {
            _implementation = implementation as BridgeUserEmailImplementation;
        }

        public async Task<BridgeLoginResponseModel> Login(BridgeLoginRequestModel model)
        {
            return await _implementation.Login(model);
        }

        public async Task<BridgeRegisterResponseModel> Register(BridgeRegisterRequestModel model)
        {
            return await _implementation.Register(model);
        }
    }
}
