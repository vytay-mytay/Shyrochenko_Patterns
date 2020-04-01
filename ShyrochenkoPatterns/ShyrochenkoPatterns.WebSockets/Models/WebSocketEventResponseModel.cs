using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.WebSockets.Models
{
    public class WebSocketEventResponseModel
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
