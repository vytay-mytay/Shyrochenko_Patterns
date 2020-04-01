using Newtonsoft.Json;
using ShyrochenkoPatterns.Models.ResponseModels.Chat;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Models.ResponseModels
{
    public class ChatResponseModel : ChatBaseResponseModel
    {
        public ChatMessageBaseResponseModel LastItem { get; set; }
    }
}
