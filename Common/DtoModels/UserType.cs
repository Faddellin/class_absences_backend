using System.Text.Json.Serialization;

namespace Common.DtoModels;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserType
{
    Student,
    Teacher,
    Dean,
    Admin
}