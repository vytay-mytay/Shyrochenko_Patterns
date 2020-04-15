using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ShyrochenkoPatterns.Common.Constants;
using ShyrochenkoPatterns.Helpers.Attributes;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.Models.ResponseModels.Bridge;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces.Bridge.Abstraction;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Validate]
    public class UsersController : _BaseApiController
    {
        private readonly Func<string, IBridgeAbstraction> _bridgeAbstractionUserPhone;
        private readonly Func<string, IBridgeAbstraction> _bridgeAbstractionUserEmail;

        public UsersController(IStringLocalizer<ErrorsResource> localizer, Func<string, IBridgeAbstraction> bridgeAbstractionEmail, Func<string, IBridgeAbstraction> bridgeAbstractionPhone)
             : base(localizer)
        {
            _bridgeAbstractionUserEmail = bridgeAbstractionEmail;
            _bridgeAbstractionUserPhone = bridgeAbstractionPhone;
        }

        // POST api/v1/users
        /// <summary>
        /// Register new user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/users
        ///     {                
        ///         "email" : "test@example.com",
        ///         "password" : "1simplepassword",
        ///         "confirmPassword" : "1simplepassword"
        ///     }
        ///
        /// </remarks>
        /// <returns>User email and info about email status, or errors with an HTTP 4xx or 500 code.</returns>
        [AllowAnonymous]
        [PreventSpam(Name = "Register")]
        [SwaggerResponse(200, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<RegisterResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost]
        [Validate]
        public async Task<IActionResult> Register([FromBody]RegisterRequestModel model)
        {
            var response = await _bridgeAbstractionUserEmail(BridgeType.UserEmail).Register(model); // use bridge

            return Json(new JsonResponse<BridgeRegisterResponseModel>(response));
        }

        #region Register_Phone

        // POST api/v1/users/phone
        /// <summary>
        /// Register new user.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/users/phone
        ///     {       
        ///         "phone" : "+44755555XXXX",
        ///         "password" : "1simplepassword",
        ///         "confirmPassword" : "1simplepassword",
        ///         "deviceToken": "device1"
        ///     }
        ///
        /// </remarks>
        /// <returns>User data, or errors with an HTTP 4xx or 500 code.</returns>
        [AllowAnonymous]
        [PreventSpam(Name = "Register")]
        [SwaggerResponse(200, ResponseMessages.SuccessfulRegistration, typeof(JsonResponse<RegisterUsingPhoneResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidCredentials, typeof(ErrorResponseModel))]
        [SwaggerResponse(422, ResponseMessages.EmailAlreadyRegistered, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [SwaggerOperation(Tags = new[] { "Users Phone" })]
        [HttpPost("Phone")]
        public async Task<IActionResult> Register([FromBody]RegisterUsingPhoneRequestModel model)
        {
            var response = await _bridgeAbstractionUserPhone(BridgeType.UserPhone).Register(model); // use bridge

            return Json(new JsonResponse<BridgeRegisterResponseModel>(response));
        }

        #endregion
    }
}