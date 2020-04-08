using Microsoft.AspNetCore.Http;
using ShyrochenkoPatterns.Models.ResponseModels;
using Stripe;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShyrochenkoPatterns.Services.Interfaces.External
{
    public interface IStripeService
    {
        Task<IdResponseModel> MakePaymentAsync(int userId, string cardToken, long amount, string currency);

        Task<IdResponseModel> AddCardAsync(int userId, string cardToken);

        Task RemoveCardAsync(int userId, string cardId);

        /// <summary>
        /// Update payment method
        /// </summary>
        /// <param name="sourceToken">Payment method token</param>
        /// <param name="customerId">Customer id</param>
        Task UpdateCustomerPaymentMethod(string sourceToken, string customerId);

        Task<List<IdResponseModel>> GetAllCardsAsync(int userId);

        Task ProcessWebhook(HttpContext httpContext, HttpRequest httpRequest);

        /// <summary>
        /// Create new stripe subscription
        /// </summary>
        /// <param name="planId">Subscriptions plan id</param>
        /// <param name="paymentMethodToken">Payment method source token</param>
        /// <param name="userId">Subscriber id</param>
        /// <returns>Subscription id</returns>
        Task<IdResponseModel> CreateSubscription(string planId, int userId);

        /// <summary>
        /// Cancel subscription
        /// </summary>
        /// <param name="subscriptionId">Subscription id</param>
        /// <param name="userId">Subscriber id</param>
        /// <returns>Canceled subscription id</returns>
        Task<IdResponseModel> CancelSubscription(string subscriptionId, int userId);

        /// <summary>
        /// Find existing user's subscription
        /// </summary>
        /// <param name="planId">Subscriptions plan id</param>
        /// <param name="customerId">Customer id</param>
        /// <returns></returns>
        Task<Subscription> FindSubscriptionByPlan(string planId, string customerId);
    }
}
