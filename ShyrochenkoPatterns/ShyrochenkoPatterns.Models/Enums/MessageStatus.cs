using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShyrochenkoPatterns.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageStatus
    {
        Sent,
        Read
    }
}
