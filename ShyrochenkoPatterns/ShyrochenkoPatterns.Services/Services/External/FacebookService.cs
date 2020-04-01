using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.Common.Utilities;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.InternalModels;
using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.RequestModels.Socials;
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

        public async Task<LoginResponseModel> ProcessRequest(FacebookWithPhoneRequestModel model)
        {
            var profile = await GetProfile(model.Token);

            var userWithFacebook = _unitOfWork.Repository<ApplicationUser>().Get(x => x.FacebookId == profile.Id)
                .Include(x => x.VerificationTokens)
                .FirstOrDefault();

            // If there is such user in DB - just return 
            if (userWithFacebook != null)
            {
                var loginResponse = await _jwtService.BuildLoginResponse(userWithFacebook);

                return loginResponse;
            }
            else if (userWithFacebook == null && model.PhoneNumber != null)
            {
                // Check if there is such user in DB, if so - add to it facebook id
                var existingUser = _unitOfWork.Repository<ApplicationUser>().Find(x => x.PhoneNumber == model.PhoneNumber);

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
                            PhoneNumber = model.PhoneNumber,
                            FacebookId = profile.Id
                        }, new JsonSerializerSettings { Formatting = Formatting.Indented });

                        await _smsService.SendVerificationCodeAsync(model.PhoneNumber, VerificationCodeType.ConfirmFacebook, data);
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

        /*  ===== WARNING =====
            There is a case when first user has an account registered with specific email;
            Second user creates an account via Facebook (that has only phone number without email) and pass the same email to request model;
            Our app find existing user with this email, link new facebook to account of first user and gives tokens of second user to first user.
        */
        public async Task<LoginResponseModel> ProcessRequest(FacebookWithEmailRequestModel model)
        {
            var profile = await GetProfile(model.Token);

            var userWithFacebook = _unitOfWork.Repository<ApplicationUser>().Get(x => x.FacebookId == profile.Id)
                .Include(x => x.VerificationTokens)
                .FirstOrDefault();

            var email = profile?.Email ?? model.Email;

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
    }
}
