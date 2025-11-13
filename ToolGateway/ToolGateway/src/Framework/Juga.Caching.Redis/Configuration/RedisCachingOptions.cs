using Juga.Caching.Common.Configuration;
using Newtonsoft.Json;

namespace Juga.Caching.Redis.Configuration;

/// <summary>
/// Redis cache konfigurasyonu için kullanılır.
/// </summary>
public class RedisCachingOptions : CachingOptions
{
    /// <summary>
    /// Configuration Section.
    /// </summary>
    public const string ConfigurationSection = "Juga:Caching:Redis";

    /// <summary>
    /// Redis Configuration Key.
    /// </summary>
    public string ConfigurationKey { get; set; } = "redisCache";
    /// <summary>
    /// Redis e bağlanmak için kullanılacak olan connection string bilgisi.
    /// </summary>
    public string ConnectionString { get; set; } = "localhost:6379,allowAdmin=true";
    /// <summary>
    /// Redis database numarasını belirtmek için kullanılır.
    /// </summary>
    public int Database { get; set; } = 0;
    /// <summary>
    /// Her bir işlem için max tekrar deneme sayısı.
    /// </summary>
    public int MaxRetries { get; set; } = 50;
    /// <summary>
    /// Her bir denemeden sonra tekrar deneme için gerekli olan milisaniye cinsinden bekleme süresi.
    /// </summary>
    public int RetryTimeout { get; set; } = 100;

    /// <summary>
    /// Serializer olarak json serializer kullanılması için kullanılır.
    /// </summary>
    public bool EnableJsonSerializer { get; set; }

    /// <summary>
    /// Jsonserializer kullanıldığı durumda NewtonsoftJson için PreserveReferencesHandling değeri. 
    /// </summary>
    public PreserveReferencesHandling PreserveReferencesHandling { get; set; } = PreserveReferencesHandling.None;
}