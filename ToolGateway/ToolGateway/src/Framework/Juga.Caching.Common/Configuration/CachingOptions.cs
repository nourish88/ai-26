using Juga.Abstractions.Caching;

namespace Juga.Caching.Common.Configuration;

/// <summary>
///     Caching katmanının genel configurasyonu için kullanılır.
/// </summary>
public class CachingOptions
{
    /// <summary>
    ///     Cache adı.
    /// </summary>
    public virtual string Name { get; set; }

    /// <summary>
    ///     Cache handlerlar arasında senkronizasyonu aktif kılmak için kullanılır.
    /// </summary>
    public virtual bool EnableCacheUpdateMode { get; set; }

    /// <summary>
    ///     Default Cache Expiration Mode
    /// </summary>
    public virtual CacheExpirationTypeEnum DefaultExpirtaionMode { get; set; }

    /// <summary>
    ///     Default Cache Expiration Timeout
    /// </summary>
    public virtual TimeSpan DefaultExpirationTimeout { get; set; }

    /// <summary>
    ///     Policy düzeyinde cache ayarları.
    /// </summary>
    public virtual CachePolicySetting PolicySettings { get; set; }

    /// <summary>
    ///     Key düzeyinde cache ayarları.
    /// </summary>
    public virtual CacheItemSetting CacheItemSettings { get; set; }
}