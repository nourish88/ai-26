using CacheManager.Core;
using Juga.Caching.Common.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Juga.Caching.InMemory.Configuration
{
    public static class InMemoryCachingServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryCache(this IServiceCollection services, IConfiguration configuration)
        {
            var inMemoryCachingOptions = new InMemoryCachingOptions();
            configuration.Bind(InMemoryCachingOptions.ConfigurationSection, inMemoryCachingOptions);

            var cacheConfiguration = GetCacheConfiguration(inMemoryCachingOptions);
            services.AddCache(configuration, cacheConfiguration, InMemoryCachingOptions.ConfigurationSection);
            return services;
        }

        private static ICacheManagerConfiguration GetCacheConfiguration(InMemoryCachingOptions cachingOptions)
        {
            return
                 CacheConfigurationBuilder.WithOptions(cachingOptions)
                .WithInMemoryOptions(cachingOptions)
                .Build();
        }
    }
}
