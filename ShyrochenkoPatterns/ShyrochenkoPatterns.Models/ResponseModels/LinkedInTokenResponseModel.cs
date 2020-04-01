using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class LinkedInTokenResponseModel
    {
        [JsonProperty(PropertyName = "access_token")]
        public string Access_token { get; set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int Expires_in { get; set; }
    }
}
