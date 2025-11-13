using CacheManager.Core;
using Juga.Caching.Common;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Framework.UnitTest")]
namespace Juga.Caching.InMemory.Configuration;

internal static class CacheConfigurationBuilderExtensions
{
    internal static ConfigurationBuilderCachePart WithInMemoryOptions(this ConfigurationBuilderCachePart configurationPart, InMemoryCachingOptions cachingOptions)
    {
        configurationPart.WithMicrosoftMemoryCacheHandle()
            .WithExpiration(cachingOptions.DefaultExpirtaionMode.ToExpirationMode(), cachingOptions.DefaultExpirationTimeout);
        if (cachingOptions.EnableJsonSerializer)
        {
            configurationPart.WithJsonSerializer();
        }
        return configurationPart;
    }
}