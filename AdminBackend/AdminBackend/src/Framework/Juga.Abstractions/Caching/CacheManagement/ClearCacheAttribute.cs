using Juga.Abstractions.Ioc;

namespace Juga.Abstractions.Caching.CacheManagement;

[AttributeUsage(AttributeTargets.Method)]
public class ClearCacheAttribute : InterceptionAttribute
{
    public ClearCacheAttribute(string key, string region = Constants.DefaultRegion
        , int keySuffixArgumentIndex = -1, string keySuffixModelPropertyName = null)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
        Key = key;
        Region = region;
        if (keySuffixArgumentIndex != -1)
            CacheKeySuffixSelector = new ValueArgumentSuffixSelector(keySuffixArgumentIndex);
        else if (!string.IsNullOrEmpty(keySuffixModelPropertyName))
            CacheKeySuffixSelector = new ModelPropertySuffixSelector(keySuffixModelPropertyName);
    }

    public string Key { get; set; }
    public string Region { get; set; }
    public ICacheKeySuffixSelector CacheKeySuffixSelector { get; set; }
}