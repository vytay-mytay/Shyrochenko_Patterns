using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ShyrochenkoPatterns.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EmailType
    {
        SuccessfulRegistration,
        ConfrimEmail,
        ResetPassword
    }
}
