using Newtonsoft.Json;
using System.Collections.Generic;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class PaymentMethodsResponseModel
    {
        [JsonProperty("paymentMethods")]
        public List<string> PaymentMethods { get; set; }
    }
}
