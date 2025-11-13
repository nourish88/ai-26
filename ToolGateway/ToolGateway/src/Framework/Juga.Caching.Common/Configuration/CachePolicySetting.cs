using Juga.Abstractions.Caching.Configuration;

namespace Juga.Caching.Common.Configuration;

/// <summary>
///     Policy düzeyinde cache expiration ayarlamak için kullanılır.
/// </summary>
public class CachePolicySetting : Dictionary<string, CacheExpirationSetting>
{
    ///// <summary>
    ///// Policy Name.
    ///// </summary>
    //public string Name { get; set; }
    ///// <summary>
    ///// Expiration ayarları.
    ///// </summary>
    //public CacheExpirationSetting ExpirationSetting { get; set; }
}