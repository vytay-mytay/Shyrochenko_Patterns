using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Identity;
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
    public class BridgeUserFacebookEmailImplementation : IBridgeImplementation
    {
        private IUnitOfWork _unitOfWork;
        private IJWTService _jwtService;
        private IFacebookService _facebookService;
        private UserManager<ApplicationUser> _userManager;

        public BridgeUserFacebookEmailImplementation(IUnitOfWork unitOfWork, IJWTService jwtService, IFacebookService facebookService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _facebookService = facebookService;
            _userManager = userManager;
        }

        public async Task<BridgeLoginResponseModel> Login(BridgeLoginRequestModel model)
        {
            var request = model as FacebookWithEmailRequestModel;
            var profile = await _facebookService.GetProfile(request.Token);

            var userWithFacebook = _unitOfWork.Repository<ApplicationUser>().Get(x => x.FacebookId == profile.Id)
                .Include(x => x.VerificationTokens)
                .FirstOrDefault();

            var email = profile?.Email ?? request.Email;

            // If there is such user in DB - just return 
            if (userWithFacebook != null)
            {
                var loginResponse = await _jwtService.BuildLoginResponse(userWithFacebook);

                return loginResponse;
            }
            else if (userWithFacebook == null && email != null)
            {
                // Check if there is such user in DB, if so - add to it facebook id
                var existingUser = _unitOfWork.Repository<ApplicationUser>().Find(x => x.Email == email);

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
                    // In other case - create new user
                    var user = CreateUserWithEmail(new RegisterWithFacebookUsingEmailInternalModel
                    {
                        Email = email,
                        FacebookId = profile.Id
                    });

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                        throw new CustomException(HttpStatusCode.BadRequest, "general", result.Errors.FirstOrDefault().Description);

                    result = await _userManager.AddToRoleAsync(user, Role.User);

                    if (!result.Succeeded)
                        throw new CustomException(HttpStatusCode.BadRequest, "general", result.Errors.FirstOrDefault().Description);

                    var loginResponse = await _jwtService.BuildLoginResponse(user);

                    return loginResponse;
                }
            }
            else
            {
                throw new CustomException(HttpStatusCode.BadRequest, "token", "There is no user with such facebook id");
            }
        }

        private ApplicationUser CreateUserWithEmail(RegisterWithFacebookUsingEmailInternalModel data)
        {
            return new ApplicationUser
            {
                Email = data.Email,
                UserName = data.Email,
                IsActive = true,
                RegistratedAt = DateTime.UtcNow,
                EmailConfirmed = false,
                FacebookId = data.FacebookId
            };
        }

        public async Task<BridgeRegisterResponseModel> Register(BridgeRegisterRequestModel model)
        {
            throw new NotImplementedException();
        }
    }
}
