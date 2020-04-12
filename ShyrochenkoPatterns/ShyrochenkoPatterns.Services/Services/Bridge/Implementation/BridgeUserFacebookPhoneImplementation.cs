using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.InternalModels;
using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using ShyrochenkoPatterns.Models.RequestModels.Socials;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;
using ShyrochenkoPatterns.Services.Interfaces;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Implementation;
using ShyrochenkoPatterns.Services.Interfaces.External;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Bridge.Implementation
{
    public class BridgeUserFacebookPhoneImplementation : IBridgeImplementation
    {
        private IUnitOfWork _unitOfWork;
        private IJWTService _jwtService;
        private IFacebookService _facebookService;
        private ISMSService _smsService;

        public BridgeUserFacebookPhoneImplementation(IUnitOfWork unitOfWork, IJWTService jwtService, IFacebookService facebookService, ISMSService smsService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _facebookService = facebookService;
            _smsService = smsService;
        }

        public async Task<BridgeLoginResponseModel> Login(BridgeLoginRequestModel model)
        {
            var request = model as FacebookWithPhoneRequestModel;
            var profile = await _facebookService.GetProfile(request.Token);

            var userWithFacebook = _unitOfWork.Repository<ApplicationUser>().Get(x => x.FacebookId == profile.Id)
                .Include(x => x.VerificationTokens)
                .FirstOrDefault();

            // If there is such user in DB - just return 
            if (userWithFacebook != null)
            {
                var loginResponse = await _jwtService.BuildLoginResponse(userWithFacebook);

                return loginResponse;
            }
            else if (userWithFacebook == null && request.PhoneNumber != null)
            {
                // Check if there is such user in DB, if so - add to it facebook id
                var existingUser = _unitOfWork.Repository<ApplicationUser>().Find(x => x.PhoneNumber == request.PhoneNumber);

                if (existingUser != null)
                {
                    existingUser.FacebookId = profile.Id;

                    _unitOfWork.Repository<ApplicationUser>().Update(existingUser);
                    _unitOfWork.SaveChanges();

                    var loginResponse = await _jwtService.BuildLoginResponse(existingUser);

                    return loginResponse;
                }
                else
                {
                    // In other case create VerificationCode with user data and send core to user
                    try
                    {
                        var data = JsonConvert.SerializeObject(new RegisterWithFacebookUsingPhoneInternalModel
                        {
                            PhoneNumber = request.PhoneNumber,
                            FacebookId = profile.Id
                        }, new JsonSerializerSettings { Formatting = Formatting.Indented });

                        await _smsService.SendVerificationCodeAsync(request.PhoneNumber, VerificationCodeType.ConfirmFacebook, data);
                    }
                    catch
                    {
                        throw new CustomException(HttpStatusCode.BadRequest, "phoneNumber", "Error while sending message");
                    }

                    throw new CustomException(HttpStatusCode.NoContent, "phoneNumber", "Verification code sent");
                }
            }
            else
            {
                throw new CustomException(HttpStatusCode.BadRequest, "token", "There is no user with such facebook id");
            }
        }

        public async Task<BridgeRegisterResponseModel> Register(BridgeRegisterRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}
