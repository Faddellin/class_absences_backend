using System.Text.Json.Serialization;

namespace Common.DtoModels;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReasonType
{
    Valid,
    Invalid,
    Unchecked
}