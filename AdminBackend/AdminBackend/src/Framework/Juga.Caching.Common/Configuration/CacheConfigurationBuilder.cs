using CacheManager.Core;

namespace Juga.Caching.Common.Configuration;

public static class CacheConfigurationBuilder
{
    public static ConfigurationBuilderCachePart WithOptions(CachingOptions cachingOptions)
    {
        var cacheBuiler = new ConfigurationBuilder(cachingOptions.Name);
        cacheBuiler.WithUpdateMode(cachingOptions.EnableCacheUpdateMode
            ? CacheUpdateMode.Up
            : CacheUpdateMode.None);
        return cacheBuiler;
    }
}