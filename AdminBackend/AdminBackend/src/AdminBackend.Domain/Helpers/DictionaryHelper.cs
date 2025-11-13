namespace AdminBackend.Domain.Helpers
{
    public static class DictionaryHelper
    {
        public static string? GetValueOrNull(this Dictionary<string,string> dictionary, string key)
        {
            dictionary.TryGetValue(key, out var value);
            return value;
        }
    }
}
