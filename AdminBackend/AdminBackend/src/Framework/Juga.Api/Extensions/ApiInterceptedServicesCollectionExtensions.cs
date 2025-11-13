using Castle.DynamicProxy;
using Juga.Caching.Common.CacheManagement;
using Juga.Data.TransactionManagement;


namespace Juga.Api.Extensions;

/// <summary>
/// Extension methods for adding intercepted services to the IServiceCollection.
/// </summary>
public static class ApiInterceptedServicesCollectionExtensions
{
    /// <summary>
    /// Adds services with interceptors to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="options">Configuration options for the API.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The IServiceCollection with the added services.</returns>
    /// <remarks>
    /// This method registers interceptors as singletons if Mediator is not enabled in the configuration.
    /// It then automatically registers services from specified assemblies.
    /// </remarks>
    public static IServiceCollection AddInterceptedServices(this IServiceCollection services)
    {

        services.AddSingleton<TransactionalInterceptor>();
        services.AddSingleton<CacheInterceptor>();
        services.AddSingleton<ClearCacheInterceptor>();
        

        return services;
    }

    /// <summary>
    /// Automatically registers services from specified assemblies with interceptors.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="options">Configuration options for the API.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <remarks>
    /// This method uses Castle DynamicProxy to create proxies for registered services. It registers services
    /// that follow the naming convention of ending with "Service" and have a corresponding interface
    /// prefixed with "I".
    /// </remarks>
 

    /// <summary>
    /// Gets the service types from the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to scan for service types.</param>
    /// <returns>A list of service types to be registered.</returns>
    internal static IEnumerable<Type> GetServiceTypesFromAssembly(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
            .ToList();
    }

    /// <summary>
    /// Registers a service and its corresponding interface with interceptors.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="implementationType">The type of the service implementation.</param>
    /// <param name="proxyGenerator">The proxy generator to create proxies.</param>
    /// <param name="options">Configuration options for the API.</param>
    /// <param name="configuration">The application configuration.</param>
    internal static void RegisterService(IServiceCollection services, Type implementationType, ProxyGenerator proxyGenerator,
        IApiOptions options, IConfiguration configuration)
    {
        var interfaceType = implementationType.GetInterfaces()
            .FirstOrDefault(i => i.Name == "I" + implementationType.Name);

        if (interfaceType != null)
        {
            services.AddScoped(implementationType);
            services.AddScoped(interfaceType, provider =>
            {
                var implementation = provider.GetRequiredService(implementationType);
                IInterceptor[] interceptors = GetInterceptors(provider, configuration, options);

                return proxyGenerator.CreateInterfaceProxyWithTarget(interfaceType, implementation, interceptors);
            });
        }
    }

    /// <summary>
    /// Gets the interceptors to be applied to the service.
    /// </summary>
    /// <param name="provider">The service provider.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="options">Configuration options for the API.</param>
    /// <remarks>If mediatr is enabled mediator pipeline will be used. Otherwise the interceptors will be in use.</remarks>
    /// <returns>An array of interceptors to be applied.</returns>
    private static IInterceptor[] GetInterceptors(IServiceProvider provider, IConfiguration configuration, IApiOptions options)
    {
        if (CommonHelpers.IsMediatrEnabled(configuration, options))
        {
            return [];
        }

        return
        [
            provider.GetRequiredService<TransactionalInterceptor>(),
            provider.GetRequiredService<CacheInterceptor>(),
            provider.GetRequiredService<ClearCacheInterceptor>()
        ];
    }
}