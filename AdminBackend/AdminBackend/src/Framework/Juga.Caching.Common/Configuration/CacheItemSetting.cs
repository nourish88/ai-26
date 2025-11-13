using Juga.Abstractions.Caching.Configuration;

namespace Juga.Caching.Common.Configuration;

/// <summary>
///     Cache düzeyinde cache expiration ayarlamak için kullanılır.
/// </summary>
public class CacheItemSetting : Dictionary<string, CacheExpirationSetting>
{
    ///// <summary>
    ///// Cache key.
    ///// </summary>
    //public string Key { get; set; }
    ///// <summary>
    ///// Expiration ayarları.
    ///// </summary>
    //public CacheExpirationSetting ExpirationSetting { get; set; }
}