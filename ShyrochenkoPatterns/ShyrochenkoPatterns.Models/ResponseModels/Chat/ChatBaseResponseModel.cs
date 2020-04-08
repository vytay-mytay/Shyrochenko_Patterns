using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Models.ResponseModels.Chat
{
    public class ChatBaseResponseModel
    {
        [JsonRequired]
        public int ChatId { get; set; }

        [JsonRequired]
        public List<UserProfileResponseModel> ChatUsers { get; set; }
    }
}
