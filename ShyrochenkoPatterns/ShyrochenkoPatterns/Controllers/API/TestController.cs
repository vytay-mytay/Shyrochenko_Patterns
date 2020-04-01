using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ShyrochenkoPatterns.Common.Constants;
using ShyrochenkoPatterns.DAL.Abstract;
using ShyrochenkoPatterns.Domain.Entities.Identity;
using ShyrochenkoPatterns.Helpers.Attributes;
using ShyrochenkoPatterns.Models.Notifications;
using ShyrochenkoPatterns.Models.RequestModels.Test;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces;
using ShyrochenkoPatterns.Services.Interfaces.External;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Controllers.API
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{api-version:apiVersion}/[controller]")]
    [Validate]
    public class TestController : _BaseApiController
    {
        private ILogger<TestController> _logger;
        private IUnitOfWork _unitOfWork;
        private ITwillioService _twillioService;
        private IFCMService _fcmService;
        private IJWTService _jwtService;
        private IUserService _userService;

        public TestController(IStringLocalizer<ErrorsResource> localizer,
            ILogger<TestController> logger,
            IUnitOfWork unitOfWork,
            ITwillioService twillioService,
            IFCMService fcmService,
            IJWTService jwtService,
            IUserService userService)
            : base(localizer)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _twillioService = twillioService;
            _fcmService = fcmService;
            _jwtService = jwtService;
            _userService = userService;
        }

        /// <summary>
        /// For Swagger UI
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("authorize")]
        public async Task<IActionResult> AuthorizeWithoutCredentials([FromBody]ShortAuthorizationRequestModel model)
        {
            ApplicationUser user = null;

            if (model.Id.HasValue)
                user = _unitOfWork.Repository<ApplicationUser>().Find(x => x.Id == model.Id);
            else if (!string.IsNullOrEmpty(model.UserName))
                user = _unitOfWork.Repository<ApplicationUser>().Find(x => x.UserName == model.UserName);

            if (user == null)
            {
                Errors.AddError("", "User is not found");
                return Errors.Error(HttpStatusCode.NotFound);
            }

            return Json(new JsonResponse<LoginResponseModel>(await _jwtService.BuildLoginResponse(user)));
        }

        // POST api/v1/test/sendSMS
        /// <summary>
        /// Send test SMS. Only for dev purposes.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/test/SMS
        ///     {                
        ///         "phone" : "+447818425330",
        ///         "text" : "text"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Sent successfully</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("SendSMS")]
        [PreventSpam(Name = "SendSMS")]
        [AllowAnonymous]
        [SwaggerResponse(200, ResponseMessages.MessageSent, typeof(JsonResponse<MessageResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        public async Task<IActionResult> SendSms([FromBody]SendTestSMSRequestModel model)
        {
            await _twillioService.SendMessageAsync(model.Phone, model.Text);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Sent")));
        }

        // POST api/v1/test/pushNotification
        /// <summary>
        /// Send Push notification to iOS or Android device
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/test/pushNotification
        ///
        /// </remarks>
        [HttpPost("PushNotification")]
        [SwaggerResponse(200, ResponseMessages.MessageSent, typeof(JsonResponse<MessageResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(401, ResponseMessages.Unauthorized, typeof(ErrorResponseModel))]
        [SwaggerResponse(403, ResponseMessages.Forbidden, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        public async Task<IActionResult> SendPush(string deviceToken, string title, string body, [FromBody]PushNotificationData data)
        {

            if (deviceToken == null)
                return Errors.BadRequest("deviceToken", "Device Token is null");

            if (title == null || body == null)
                return Errors.BadRequest("title", "Title/Body is null");

            await _fcmService.SendPushNotification(deviceToken, new PushNotification(title, body, data), testMode: true);

            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("Sent successfully")));
        }

        // DELETE api/v1/test/DeleteAccount
        /// <summary>
        /// Hard delete user from db
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/v1/test/DeleteAccount
        ///
        /// </remarks>
        /// <response code="200">Successfully deleted</response>
        /// <response code="400">Bad Request</response>    
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("DeleteAccount")]
        [SwaggerResponse(200, ResponseMessages.RequestSuccessful, typeof(JsonResponse<MessageResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(401, ResponseMessages.Unauthorized, typeof(ErrorResponseModel))]
        [SwaggerResponse(403, ResponseMessages.Forbidden, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        public IActionResult DeleteAccount([FromQuery]int userId)
        {
            if (userId <= 0)
                return Errors.BadRequest("userId", "Invalid user id");

            _userService.HardDeleteUser(userId);
            return Json(new JsonResponse<MessageResponseModel>(new MessageResponseModel("User has been deleted")));
        }
    }
}