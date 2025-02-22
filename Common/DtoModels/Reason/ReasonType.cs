using System.Text.Json.Serialization;

namespace Common.DtoModels.Reason;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReasonType
{
    Valid,
    Invalid,
    Unchecked
}