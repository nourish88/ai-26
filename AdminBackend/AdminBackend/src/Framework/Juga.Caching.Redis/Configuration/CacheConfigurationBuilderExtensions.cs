using CacheManager.Core;
using Juga.Caching.Common;

namespace Juga.Caching.Redis.Configuration;

internal static class CacheConfigurationBuilderExtensions
{
    internal static ConfigurationBuilderCachePart WithRedisOptions(this ConfigurationBuilderCachePart configurationPart, RedisCachingOptions cachingOptions)
    {
        configurationPart.WithRedisConfiguration(cachingOptions.ConfigurationKey, cachingOptions.ConnectionString, cachingOptions.Database)
            .WithRedisCacheHandle(cachingOptions.ConfigurationKey)
            .WithExpiration(cachingOptions.DefaultExpirtaionMode.ToExpirationMode(), cachingOptions.DefaultExpirationTimeout);
        configurationPart.WithMaxRetries(cachingOptions.MaxRetries);
        configurationPart.WithRetryTimeout(cachingOptions.RetryTimeout);
        if (cachingOptions.EnableJsonSerializer)
        {
            var serializationSettings = new Newtonsoft.Json.JsonSerializerSettings() { PreserveReferencesHandling = cachingOptions.PreserveReferencesHandling };
            configurationPart.WithJsonSerializer(serializationSettings, serializationSettings);
        }
        return configurationPart;
    }
}