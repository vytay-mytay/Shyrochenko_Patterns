﻿using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Abstraction;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Implementation;
using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Abstraction.Bridge
{
    public class BridgeUserPhone : IBridgeAbstraction
    {
        private Func<string, IBridgeImplementation> _implementation;

        public BridgeUserPhone(Func<string, IBridgeImplementation> implementation)
        {
            _implementation = implementation;
        }

        public async Task<BridgeLoginResponseModel> Login(BridgeLoginRequestModel model)
        {
            return await _implementation(BridgeType.FacebookPhone).Login(model);
        }

        public async Task<BridgeRegisterResponseModel> Register(BridgeRegisterRequestModel model)
        {
            return await _implementation(BridgeType.FacebookPhone).Register(model);
        }
    }
}
