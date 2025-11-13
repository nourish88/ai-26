using System.Text.Json;

namespace Juga.Client.Extensions;

public static class QueryStringExtensions
{
    public static string AddQueryString(this string url, object parameters)
    {
        var serialized = JsonSerializer.Serialize(parameters);
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, string>>(serialized);
        var result = deserialized.Where(p => p.Value != null)
            .Select(kvp => kvp.Key.ToString() + "=" + Uri.EscapeDataString(kvp.Value))
            .Aggregate((p1, p2) => p1 + "&" + p2);
        return $"{url}?{result}";
    }
}