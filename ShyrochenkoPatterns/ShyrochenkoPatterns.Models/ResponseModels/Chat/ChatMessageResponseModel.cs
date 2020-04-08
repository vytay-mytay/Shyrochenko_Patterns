using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Models.ResponseModels.Chat
{
    public class ChatMessageResponseModel : ChatMessageBaseResponseModel
    {
        public int? UnreadMesagesInChat { get; set; }

        public int? AllUnreadMesages { get; set; }
    }
}
