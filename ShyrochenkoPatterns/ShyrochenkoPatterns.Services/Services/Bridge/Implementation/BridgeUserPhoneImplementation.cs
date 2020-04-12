using Microsoft.AspNetCore.Identity;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.RequestModels.Bridge;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;
using ShyrochenkoPatterns.Services.Interfaces;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Implementation;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.Bridge.Implementation
{
    public class BridgeUserPhoneImplementation : IBridgeImplementation
    {
        private IUnitOfWork _unitOfWork;
        private IJWTService _jwtService;
        private UserManager<ApplicationUser> _userManager;
        private IAccountService _accountService;

        public BridgeUserPhoneImplementation(IUnitOfWork unitOfWork, IJWTService jwtService, UserManager<ApplicationUser> userManager, IAccountService accountService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _userManager = userManager;
            _accountService = accountService;
        }

        public async Task<BridgeLoginResponseModel> Login(BridgeLoginRequestModel model)
        {
            var request = model as LoginWithPhoneRequestModel;
            var user = _unitOfWork.Repository<ApplicationUser>().Find(x => x.PhoneNumber == request.PhoneNumber);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new CustomException(HttpStatusCode.BadRequest, "credentials", "Invalid credentials");

            if (!user.PhoneNumberConfirmed)
                throw new CustomException(HttpStatusCode.BadRequest, "phoneNumber", "PhoneNumber is not confirmed");

            if (user.IsDeleted)
                throw new CustomException(HttpStatusCode.BadRequest, "general", "Your account was deleted by admin, to know more please contact administration.");

            if (!user.IsActive)
                throw new CustomException(HttpStatusCode.MethodNotAllowed, "general", "Your account was blocked. For more information please email to following address: ");

            return await _jwtService.BuildLoginResponse(user, request.AccessTokenLifetime);
        }

        public async Task<BridgeRegisterResponseModel> Register(BridgeRegisterRequestModel model)
        {
            var request = model as RegisterRequestModel;
            request.Email = request.Email.Trim().ToLower();

            ApplicationUser user = _unitOfWork.Repository<ApplicationUser>().Find(x => x.Email.ToLower() == request.Email);

            if (user != null && user.EmailConfirmed)
                throw new CustomException(HttpStatusCode.UnprocessableEntity, "email", "Email is already registered");

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = request.Email,
                    UserName = request.Email,
                    IsActive = true,
                    RegistratedAt = DateTime.UtcNow,
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                    throw new CustomException(HttpStatusCode.BadRequest, "general", result.Errors.FirstOrDefault().Description);

                result = await _userManager.AddToRoleAsync(user, Role.User);

                if (!result.Succeeded)
                    throw new CustomException(HttpStatusCode.BadRequest, "general", result.Errors.FirstOrDefault().Description);
            }

            try
            {
                await _accountService.SendConfirmEmailLink(user);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                throw;
            }

            return new RegisterResponseModel { Email = user.Email };
        }
    }
}
