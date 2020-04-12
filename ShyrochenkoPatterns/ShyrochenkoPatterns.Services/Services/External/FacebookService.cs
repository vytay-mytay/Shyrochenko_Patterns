using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.Common.Utilities;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.InternalModels;
using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Services.Interfaces;
using ShyrochenkoPatterns.Services.Interfaces.External;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Services.External
{
    public class FacebookService : IFacebookService
    {
        private IConfiguration _configuration = null;
        private HttpClient _httpClient;
        private HashUtility _hashUtility;
        private UserManager<ApplicationUser> _userManager;
        private IUnitOfWork _unitOfWork;
        private ISMSService _smsService;
        private IJWTService _jwtService;

        public FacebookService(IConfiguration configuration, HttpClient httpClient, HashUtility hashUtility, UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork, ISMSService smsService, IJWTService jwtService)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _hashUtility = hashUtility;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _smsService = smsService;
            _jwtService = jwtService;
        }

        private ApplicationUser CreateUserWithPhone(RegisterWithFacebookUsingPhoneInternalModel data)
        {
            return new ApplicationUser
            {
                PhoneNumber = data.PhoneNumber,
                UserName = data.PhoneNumber,
                IsActive = true,
                RegistratedAt = DateTime.UtcNow,
                PhoneNumberConfirmed = false,
                FacebookId = data.FacebookId
            };
        }

        public async Task<LoginResponseModel> ConfirmFacebookRegistration(ConfirmPhoneRequestModel model)
        {
            var code = _unitOfWork.Repository<VerificationToken>()
                .Find(x => !x.IsUsed && x.IsValid && x.Type == VerificationCodeType.ConfirmFacebook && x.TokenHash == _hashUtility.GetHash(model.Code));

            if (code == null)
                throw new CustomException(HttpStatusCode.BadRequest, "code", "SMS code is not valid. Add correct code or re-send it");

            // Parse and create user
            var userData = JsonConvert.DeserializeObject<RegisterWithFacebookUsingPhoneInternalModel>(code.Data);

            var user = CreateUserWithPhone(userData);

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
                throw new CustomException(HttpStatusCode.BadRequest, "general", result.Errors.FirstOrDefault().Description);

            result = await _userManager.AddToRoleAsync(user, Role.User);

            if (!result.Succeeded)
                throw new CustomException(HttpStatusCode.BadRequest, "general", result.Errors.FirstOrDefault().Description);

            code.IsUsed = true;

            _unitOfWork.Repository<VerificationToken>().Update(code);
            _unitOfWork.SaveChanges();

            var loginResponse = await _jwtService.BuildLoginResponse(user);

            return loginResponse;

        }

        public async Task<FBProfileResponseModel> GetProfile(string token)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://graph.facebook.com/v3.2/me?access_token={token}&fields=id,email,first_name,last_name&client_secret={_configuration["Authentication:Facebook:AppSecret"]}&format=json");

                // Will throw an exception if not successful
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<FBProfileResponseModel>(content);

                if (result == null)
                    throw new ArgumentException("Invalid token");

                return result;
            }
            catch (Exception ex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "token", "Facebook Token is invalid");
            }
        }

        /*  ===== WARNING =====
            There is a case when first user has an account registered with specific email;
            Second user creates an account via Facebook (that has only phone number without email) and pass the same email to request model;
            Our app find existing user with this email, link new facebook to account of first user and gives tokens of second user to first user.
        */

    }
}
