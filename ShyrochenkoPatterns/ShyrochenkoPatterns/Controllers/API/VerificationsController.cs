using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.Common.Constants;
using ShyrochenkoPatterns.Common.Extensions;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Helpers.Attributes;
using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using System.Web;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Validate]
    public class VerificationsController : _BaseApiController
    {
        private UserManager<ApplicationUser> _userManager;
        private IAccountService _accountService;
        private ISMSService _sMSService;
        private IConfiguration _configuration = null;
        private ICallService _callService;

        public VerificationsController(IStringLocalizer<ErrorsResource> localizer, UserManager<ApplicationUser> userManager, IAccountService accountService, ISMSService sMSService, IConfiguration configuration, ICallService callService)
              : base(localizer)
        {
            _userManager = userManager;
            _accountService = accountService;
            _sMSService = sMSService;
            _configuration = configuration;
            _callService = callService;
        }

        // PUT api/v1/verifications/email
        /// <summary>
        /// Confirm user email
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/verifications/email
        ///     {     
        ///         "email" : "test@email.com",
        ///         "token": "some token"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and login response, or HTTP 400 with errors</returns>
        [AllowAnonymous]
        [PreventSpam(Name = "ConfirmEmail")]
        [ProducesResponseType(typeof(JsonResponse<LoginResponseModel>), 200)]
        [SwaggerResponse(200, ResponseMessages.LinkSent, typeof(JsonResponse<LoginResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPut("Email")]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailRequestModel model)
        {
            var response = await _accountService.ConfirmEmail(model);

            return Json(new JsonResponse<LoginResponseModel>(response));
        }

        // POST api/v1/verifications/password
        /// <summary>
        /// Forgot password - Send link to change password on user email.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/verifications/password
        ///     {                
        ///        "email": "test@email.com"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and message if link sended, or HTTP 400 with errors</returns> 
        [AllowAnonymous]
        [PreventSpam(Name = "ForgotPassword")]
        [SwaggerResponse(200, ResponseMessages.LinkSent, typeof(JsonResponse<MessageResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.EmailInvalidOrNotConfirmed, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("Password")]
        public async Task<IActionResult> ForgotPassword([FromBody]EmailRequestModel model)
        {
            await _accountService.SendPasswordRestorationLink(model.Email);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("If we found this email address in our database we have sent you password reset instructions by email")));
        }

        // POST api/v1/verifications/token
        /// <summary>
        /// Forgot password - Check if token is invalid or expired
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/verifications/token
        ///     {     
        ///         "email" : "test@email.com",
        ///         "token": "some token"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and message if token is checked, or HTTP 400 with errors</returns>
        [AllowAnonymous]
        [PreventSpam(Name = "CheckResetPasswordToken")]
        [SwaggerResponse(200, ResponseMessages.LinkSent, typeof(JsonResponse<CheckResetPasswordTokenResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("Token")]
        public async Task<IActionResult> CheckResetPasswordToken([FromBody]CheckResetPasswordTokenRequestModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            var token = HttpUtility.UrlDecode(model.Token).Replace(" ", "+");

            return Json(new JsonResponse<CheckResetPasswordTokenResponseModel>(new CheckResetPasswordTokenResponseModel
            {
                IsValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token)
            }));
        }

        // PUT api/v1/verifications/password
        /// <summary>
        /// Forgot password - Change user password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/verifications/password
        ///     {     
        ///        "email" : "test@email.com",
        ///        "token": "some token",
        ///        "password" : "1simplepassword",
        ///        "confirmPassword" : "1simplepassword" 
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and message if link sended, or HTTP 400 with errors</returns>
        /// <returns>A user info with an HTTP 200, or errors with an HTTP 500.</returns>  
        [AllowAnonymous]
        [PreventSpam(Name = "ResetPassword")]
        [SwaggerResponse(200, ResponseMessages.RequestSuccessful, typeof(JsonResponse<LoginResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.EmailInvalidOrNotConfirmed, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPut("Password")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequestModel model)
        {
            var response = await _accountService.ResetPassword(model);

            return Json(new JsonResponse<LoginResponseModel>(response));
        }

        #region Register_Phone

        // POST api/v1/verifications/phone/password
        /// <summary>
        /// Send SMS with confirmation code to specified phone number so that user can restore password
        /// </summary>
        /// <remarks>
        /// 
        /// Sample request:
        ///
        ///     POST api/v1/verifications/restore
        ///     {
        ///         "phoneNumber": "+44755555XXXX"
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 with success message or HTTP 40X or 500 with error message</returns>
        [AllowAnonymous]
        [PreventSpam(Name = "ForgotPassword")]
        [SwaggerResponse(200, ResponseMessages.MessageSent, typeof(JsonResponse<MessageResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [Validate]
        [SwaggerOperation(Tags = new[] { "Verifications Phone" })]
        [HttpPost("Phone/Password")]
        public async Task<IActionResult> ForgotPassword([FromBody]PhoneNumberRequestModel model)
        {
            await _accountService.SendPasswordRestorationCodeAsync(model.PhoneNumber);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("If we found this phone number in our database we have sent you password reset code")));
        }

        // PUT api/v1/verifications/phone
        /// <summary>
        /// Confirm user phone number and finish registration
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/verifications/phone
        ///     {  
        ///         "phoneNumber" : "+44755555XXXX",
        ///         "code" : "1111"
        ///     }
        ///
        /// </remarks>
        /// <returns>A user info with an HTTP 200, or errors with an HTTP 500.</returns>  
        [AllowAnonymous]
        [PreventSpam(Name = "ConfirmPhone")]
        [SwaggerResponse(200, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<LoginResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [SwaggerOperation(Tags = new[] { "Verifications Phone" })]
        [HttpPut("Phone")]
        public async Task<IActionResult> ConfirmPhone([FromBody]ConfirmPhoneRequestModel model)
        {
            var response = await _accountService.ConfirmPhone(model);

            // there is _unitOfWork.SaveChanges call inside BuildLoginResponse
            return Json(new JsonResponse<LoginResponseModel>(response));
        }

        // PUT api/v1/verifications/password
        /// <summary>
        /// Forgot password - Change user password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT api/v1/verifications/password
        ///     {     
        ///        "phoneNumber" : "test@email.com",
        ///        "token": "some token",
        ///        "password" : "1simplepassword",
        ///        "confirmPassword" : "1simplepassword" 
        ///     }
        ///
        /// </remarks>
        /// <returns>HTTP 200 and message if link sended, or HTTP 400 with errors</returns>
        [AllowAnonymous]
        [PreventSpam(Name = "ResetPassword")]
        [SwaggerResponse(200, ResponseMessages.RequestSuccessful, typeof(JsonResponse<LoginResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.EmailInvalidOrNotConfirmed, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [SwaggerOperation(Tags = new[] { "Verifications Phone" })]
        [HttpPut("Phone/Password")]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordWithPhoneRequestModel model)
        {
            var response = await _accountService.ResetPassword(model);

            return Json(new JsonResponse<LoginResponseModel>(response));
        }

        #endregion





        // send code to phone
        [HttpGet("Code")]
        public async Task<IActionResult> TwilioSendCode()
        {
            var user = await _userManager.FindByIdAsync(User.GetUserId().ToString());
            await _sMSService.SendVerificationCodeAsync(user, user.PhoneNumber, Models.Enums.VerificationCodeType.Confirm);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Code sent")));
        }

        //make call
        [HttpGet("MekeCall")]
        public async Task<IActionResult> MakeCall()
        {
            await _callService.VerificationCall(await _userManager.FindByIdAsync(User.GetUserId().ToString()));

            return Ok();
        }
    }
}