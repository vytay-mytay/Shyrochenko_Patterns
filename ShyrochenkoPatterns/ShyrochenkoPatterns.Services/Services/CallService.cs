using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Services.Interfaces;
using ShyrochenkoPatterns.Services.Interfaces.External;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace ShyrochenkoPatterns.Services.Services
{
    public class CallService : ICallService
    {
        private ITwillioService _twillioService;

        public CallService(ITwillioService twillioService)
        {
            _twillioService = twillioService;
        }
        public async Task VerificationCall(ApplicationUser user)
        {
            var call = (CallResource)await _twillioService.Call(user.PhoneNumber);

            var test = call.Sid;
        }
    }
}
