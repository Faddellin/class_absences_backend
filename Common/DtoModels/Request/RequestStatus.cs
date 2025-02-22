using System.Text.Json.Serialization;

namespace Common.DtoModels.Request;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RequestStatus
{
    Checking,
    Confirmed,
    Rejected
}