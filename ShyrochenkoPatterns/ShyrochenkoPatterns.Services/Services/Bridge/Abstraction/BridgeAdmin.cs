using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Abstraction;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Implementation;
using ShyrochenkoPatterns.Services.Services.Bridge.Implementation;
using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Abstraction.Bridge
{
    public class BridgeAdmin : IBridgeAbstraction
    {
        private BridgeAdminImplementation _implementation;

        public BridgeAdmin(IBridgeImplementation implementation)
        {
            _implementation = implementation as BridgeAdminImplementation;
        }

        public async Task<BridgeLoginResponseModel> Login(BridgeLoginRequestModel model)
        {
            return (await _implementation.Login(model)) as LoginResponseModel;
        }

        public async Task<BridgeRegisterResponseModel> Register(BridgeRegisterRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}
