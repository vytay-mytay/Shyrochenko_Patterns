using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class LinkedInService : ILinkedInService
    {
        private IConfiguration _configuration;
        private HttpClient _httpClient;
        private HashUtility _hashUtility;
        private UserManager<ApplicationUser> _userManager;
        private IUnitOfWork _unitOfWork;
        private ISMSService _smsService;
        private IJWTService _jwtService;

        public LinkedInService(IConfiguration configuration, HttpClient httpClient, HashUtility hashUtility, UserManager<ApplicationUser> userManager,
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

        public async Task<LIProfileResponseModel> GetProfile(string code, string redirectUri, bool emailRequest = false)
        {
            try
            {
                // Get oauth token
                var token = await GetAccessToken(code, redirectUri);

                var response = await _httpClient.GetAsync($"https://api.linkedin.com/v2/me?oauth2_access_token={token.Access_token}&projection=(id,firstName,lastName)");
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<LIProfileResponseModel>(content);

                // Check id
                if (string.IsNullOrEmpty(result.Id))
                    throw new ArgumentException("Invalid response");

                // Get email if there is such flag
                if (emailRequest)
                {
                    response = await _httpClient.GetAsync($"https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))&oauth2_access_token={token.Access_token}");

                    if (response.IsSuccessStatusCode)
                    {
                        var emailContent = await response.Content.ReadAsStringAsync();
                        var email = JObject.Parse(emailContent);
                        result.Email = email["elements"][0]["handle~"]["emailAddress"].ToString();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new CustomException(HttpStatusCode.BadRequest, "token", "Linkein token is invalid");
            }
        }

        public async Task<LoginResponseModel> ProcessRequest(LinkedInWithEmailRequestModel model)
        {
            var profile = await GetProfile(model.Token, model.RedirectUri, string.IsNullOrEmpty(model.Email));

            // If we didn`t get email from profile, get it from model
            var email = profile?.Email ?? model?.Email ?? null;

            var userWithLinkedIn = _unitOfWork.Repository<ApplicationUser>().Get(x => x.LinkedInId == profile.Id)
                .Include(x => x.VerificationTokens)
                .FirstOrDefault();

            // If there is such user in DB - just return 
            if (userWithLinkedIn != null)
            {
                var loginResponse = await _jwtService.BuildLoginResponse(userWithLinkedIn);

                return loginResponse;
            }
            else if (userWithLinkedIn == null && email != null)
            {
                // Check if there is such user in DB, if so - add to it google id
                var existingUser = _unitOfWork.Repository<ApplicationUser>().Find(x => x.Email == email);

                if (existingUser != null)
                {
                    existingUser.LinkedInId = profile.Id;

                    _unitOfWork.Repository<ApplicationUser>().Update(existingUser);
                    _unitOfWork.SaveChanges();

                    var loginResponse = await _jwtService.BuildLoginResponse(existingUser);

                    return loginResponse;
                }
                else
                {
                    // In other case - create new user
                    var user = new ApplicationUser
                    {
                        Email = email,
                        UserName = email,
                        IsActive = true,
                        RegistratedAt = DateTime.UtcNow,
                        EmailConfirmed = false,
                        GoogleId = profile.Id
                    };

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
                throw new CustomException(HttpStatusCode.BadRequest, "token", "There is no user with such linkedin id");
            }
        }

        public async Task<LoginResponseModel> ProcessRequest(LinkedInWithPhoneRequestModel model)
        {
            var profile = await GetProfile(model.Token, model.RedirectUri);

            var userWithGoogle = _unitOfWork.Repository<ApplicationUser>().Get(x => x.LinkedInId == profile.Id)
                .Include(x => x.VerificationTokens)
                .FirstOrDefault();

            // If there is such user in DB - just return 
            if (userWithGoogle != null)
            {
                var loginResponse = await _jwtService.BuildLoginResponse(userWithGoogle);

                return loginResponse;
            }
            else if (userWithGoogle == null && model.PhoneNumber != null)
            {
                var existingUser = _unitOfWork.Repository<ApplicationUser>().Find(x => x.PhoneNumber == model.PhoneNumber);

                if (existingUser != null)
                {
                    existingUser.LinkedInId = profile.Id;

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
                        var data = JsonConvert.SerializeObject(new RegisterWithSocialsUsingPhoneInternalModel
                        {
                            PhoneNumber = model.PhoneNumber,
                            SocialId = profile.Id
                        }, new JsonSerializerSettings { Formatting = Formatting.Indented });

                        await _smsService.SendVerificationCodeAsync(model.PhoneNumber, VerificationCodeType.ConfirmLinkedIn, data);
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
                throw new CustomException(HttpStatusCode.BadRequest, "token", "There is no user with such google id");
            }
        }

        public async Task<LoginResponseModel> ConfrimRegistration(ConfirmPhoneRequestModel model)
        {
            var code = _unitOfWork.Repository<VerificationToken>()
                .Find(x => !x.IsUsed && x.IsValid && x.Type == VerificationCodeType.ConfirmLinkedIn && x.TokenHash == _hashUtility.GetHash(model.Code));

            if (code == null)
                throw new CustomException(HttpStatusCode.BadRequest, "code", "SMS code is not valid. Add correct code or re-send it");

            // Parse and create user
            var userData = JsonConvert.DeserializeObject<RegisterWithSocialsUsingPhoneInternalModel>(code.Data);

            var user = new ApplicationUser
            {
                PhoneNumber = userData.PhoneNumber,
                UserName = userData.PhoneNumber,
                IsActive = true,
                RegistratedAt = DateTime.UtcNow,
                PhoneNumberConfirmed = false,
                LinkedInId = userData.SocialId
            };

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

        private async Task<LinkedInTokenResponseModel> GetAccessToken(string code, string redirectUri)
        {
            var response = await _httpClient.GetAsync($"https://www.linkedin.com/oauth/v2/accessToken?grant_type=authorization_code&code={code}&redirect_uri={redirectUri}&client_id={_configuration["Authentication:LinkedIn:ClientId"]}&client_secret={_configuration["Authentication:LinkedIn:ClientSecret"]}");
            response.EnsureSuccessStatusCode();

            string content = await response.Content.ReadAsStringAsync();

            var token = JsonConvert.DeserializeObject<LinkedInTokenResponseModel>(content);

            if (token == null)
                throw new ArgumentException("Invalid token");

            return token;
        }
    }
}
