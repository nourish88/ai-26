using System.Text.Json.Serialization;

namespace Juga.Abstractions.Application.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResultType
{
    Ok,
    Unexpected,
    NotFound,
    Unauthorized,
    Invalid
}