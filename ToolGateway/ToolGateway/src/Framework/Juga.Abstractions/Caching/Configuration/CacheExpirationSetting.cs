namespace Juga.Abstractions.Caching.Configuration;

public class CacheExpirationSetting
{
    /// <summary>
    /// Cache expiration mode.
    /// </summary>
    public CacheExpirationTypeEnum ExpirationMode { get; set; }

    /// <summary>
    /// Cache expiration zamanı.
    /// </summary>
    public TimeSpan ExpirationTime { get; set; }
}