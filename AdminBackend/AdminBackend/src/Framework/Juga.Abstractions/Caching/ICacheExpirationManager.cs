using Juga.Abstractions.Caching.Configuration;

namespace Juga.Abstractions.Caching;

/// <summary>
/// Cache nesnelerinin expirationlarını yönetmek için kullanılır.
/// </summary>
public interface ICacheExpirationManager
{
    /// <summary>
    /// Cache nesnesinin expiration ayarlarını belirler.
    /// </summary>
    /// <param name="policy">Cache Policy</param>
    /// <param name="key">Cache Key</param>
    /// <returns>Cache expiration ayarı</returns>
    CacheExpirationSetting GetCacheExpirationSetting(string key, string policy);

    /// <summary>
    /// Cache nesnesinin expiration ayarlarını belirler.
    /// </summary>
    /// <param name="cacheItem">Cache bilgileri</param>
    /// <returns>Cache expiration ayarı</returns>
    CacheExpirationSetting GetCacheExpirationSetting(ICacheItem cacheItem);
}