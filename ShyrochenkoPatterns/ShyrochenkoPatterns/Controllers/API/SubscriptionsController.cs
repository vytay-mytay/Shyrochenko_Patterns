using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ShyrochenkoPatterns.Common.Constants;
using ShyrochenkoPatterns.Common.Extensions;
using ShyrochenkoPatterns.Helpers.Attributes;
using ShyrochenkoPatterns.Models.RequestModels;
using ShyrochenkoPatterns.Models.ResponseModels;
using ShyrochenkoPatterns.ResourceLibrary;
using ShyrochenkoPatterns.Services.Interfaces.External;
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
    public class SubscriptionsController : _BaseApiController
    {
        private IBraintreeService _braintreeService;
        private IStripeService _stripeService;
        private ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(IStringLocalizer<ErrorsResource> localizer,
            IBraintreeService braintreeService,
            IStripeService stripeService,
            ILogger<SubscriptionsController> logger)
            : base(localizer)
        {
            _braintreeService = braintreeService;
            _stripeService = stripeService;
            _logger = logger;
        }

        #region Braintree

        // POST api/v1/Subscriptions/Braintree
        /// <summary>
        /// Create Braintree subscription by associating payment method with specified plan
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/Subscriptions/Braintree
        ///     {
        ///         "paymentMethodToken": "jd68slm",
        ///         "planId": "example_plan_id"
        ///     }
        ///
        /// </remarks>
        /// <param name="model">Token of the payment method and id of the plan</param>
        /// <returns>HTTP 200 with id of the created subscription or errors with an HTTP 40x or 500 code.</returns>
        [ProducesResponseType(typeof(JsonResponse<IdResponseModel>), 200)]
        [SwaggerResponse(200, ResponseMessages.RequestSuccessful, typeof(JsonResponse<IdResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(401, ResponseMessages.Unauthorized, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("Braintree")]
        public async Task<IActionResult> CreateBraintreeSubscription([FromBody]BraintreeSubscriptionRequestModel model)
        {
            var successPaymentResult = await _braintreeService.CreateSubscriptionAsync(model.PaymentMethodToken, model.PlanId);

            return Json(new JsonResponse<IdResponseModel>(new IdResponseModel { Id = successPaymentResult.Target.Id }));
        }

        // POST api/v1/Subscriptions/Braintree/Webhook
        /// <summary>
        /// Endpoint to receive and process Braintree's subscriptions webhooks
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/Subscriptions/Braintree/SubscriptionWebhook
        ///     {
        ///         "bt_signature": "example_bt_signature",
        ///         "bt_payload": "example_bt_payload"
        ///     }
        ///
        /// </remarks>
        /// <param name="model">Braintree signatuse and payload</param>
        /// <returns>HTTP 200 with received notification kind or errors with an HTTP 40x or 500 code.</returns>
        [SwaggerResponse(200, ResponseMessages.RequestSuccessful, typeof(JsonResponse<string>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [AllowAnonymous]
        [HttpPost("Braintree/SubscriptionWebhook")]
        public async Task<IActionResult> TestBraintreeSubscriptionWebhook([FromForm]BraintreeSubscriptionWebhookRequestModel model)
        {
            var webhookResult = _braintreeService.ProcessSubscriptionWebhook(model.bt_signature, model.bt_payload);

            System.Diagnostics.Trace.WriteLine($"Braintree/Webhook -> ({DateTime.Now.ToString("G")}) {webhookResult}");

            return Json(new JsonResponse<string>(webhookResult));
        }

        #endregion

        #region Stripe

        /// <summary>
        /// Create Stripe subscription with specified plan
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST api/v1/Subscriptions/Stripe
        ///     {
        ///         "planId": "example_plan_id"
        ///     }
        ///
        /// </remarks>
        /// <param name="model">Plan id</param>
        /// <returns>HTTP 200 with id of the created subscription or errors with an HTTP 40x or 500 code.</returns>
        [SwaggerResponse(200, ResponseMessages.RequestSuccessful, typeof(JsonResponse<IdResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(401, ResponseMessages.Unauthorized, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpPost("Stripe")]
        [Validate]
        public async Task<IActionResult> CreateStripeSubscription([FromBody]StripeCreateSubscriptionRequestModel model)
        {
            var result = await _stripeService.CreateSubscription(model.PlanId, User.GetUserId());

            return Json(new JsonResponse<IdResponseModel>(result));
        }

        /// <summary>
        /// Cancel Stripe subscription
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE api/v1/Subscriptions/Stripe/subscriptionId
        ///
        /// </remarks>
        /// <param name="model">Subscription id</param>
        /// <returns>HTTP 200 with id of the canceled subscription or errors with an HTTP 40x or 500 code.</returns>
        [SwaggerResponse(200, ResponseMessages.RequestSuccessful, typeof(JsonResponse<IdResponseModel>))]
        [SwaggerResponse(400, ResponseMessages.InvalidData, typeof(ErrorResponseModel))]
        [SwaggerResponse(401, ResponseMessages.Unauthorized, typeof(ErrorResponseModel))]
        [SwaggerResponse(500, ResponseMessages.InternalServerError, typeof(ErrorResponseModel))]
        [HttpDelete("Stripe")]
        [Validate]
        public async Task<IActionResult> CancelStripeSubscription([FromBody]StripeSubscriptionRequestModel model)
        {
            var result = await _stripeService.CancelSubscription(model.SubscriptionId, User.GetUserId());

            return Json(new JsonResponse<IdResponseModel>(result));
        }

        #endregion
    }
}