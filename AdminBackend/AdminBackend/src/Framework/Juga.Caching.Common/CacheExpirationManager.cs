using Juga.Abstractions.Caching;
using Juga.Abstractions.Caching.Configuration;
using Juga.Caching.Common.Configuration;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Framework.UnitTest")]
namespace Juga.Caching.Common;

internal class CacheExpirationManager : ICacheExpirationManager
{
    private readonly CachingOptions _cachingOptions;
    public CacheExpirationManager(IOptions<CachingOptions> cachingOptions)
    {
        _cachingOptions = cachingOptions.Value;
    }
    public CacheExpirationSetting GetCacheExpirationSetting(string key, string policy)
    {
        CacheExpirationSetting cacheExpirationSetting = GetCacheExpirationSetting(_cachingOptions.CacheItemSettings, key);
        if (cacheExpirationSetting == null)
        {
            cacheExpirationSetting = GetCacheExpirationSetting(_cachingOptions.PolicySettings, policy);
        }

        if (cacheExpirationSetting == null)
        {
            cacheExpirationSetting = GetDefaultSettings();
        }
        return cacheExpirationSetting;
    }

    public CacheExpirationSetting GetCacheExpirationSetting(ICacheItem cacheItem)
    {
        if (cacheItem == null)
        {
            return GetDefaultSettings();
        }
        else if (cacheItem.Expire != default(TimeSpan))
        {
            return new CacheExpirationSetting() { ExpirationMode = cacheItem.ExpirationMode, ExpirationTime = cacheItem.Expire };
        }
        else
        {
            return GetCacheExpirationSetting(cacheItem.Key, cacheItem.Policy);
        }
    }

    private CacheExpirationSetting GetCacheExpirationSetting(Dictionary<string, CacheExpirationSetting> cacheExpirationSettings, string key)
    {
        CacheExpirationSetting cacheExpirationSetting = null;
        if (string.IsNullOrEmpty(key))
        {
            return cacheExpirationSetting;
        }
        cacheExpirationSettings.TryGetValue(key, out cacheExpirationSetting);
        return cacheExpirationSetting;
    }

    private CacheExpirationSetting GetDefaultSettings()
    {
        return new CacheExpirationSetting()
        {
            ExpirationMode = _cachingOptions.DefaultExpirtaionMode,
            ExpirationTime = _cachingOptions.DefaultExpirationTimeout
        };
    }
}