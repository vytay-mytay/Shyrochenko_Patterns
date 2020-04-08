using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ShyrochenkoPatterns.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RequestType
    {
        GET,
        POST
    }
}
