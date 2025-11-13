using Castle.DynamicProxy;
using FluentValidation;
using Juga.Application.Pipelines.Caching;
using Juga.Application.Pipelines.RequestResponse;
using Juga.Application.Pipelines.Transaction;
using Juga.Application.Pipelines.Validation;
using Juga.Data.Interceptors;

namespace Juga.Api.Extensions;

    public static class ServiceRegistrationsExtensions
    {
        public static IServiceCollection RegisterFluentValidation(this IServiceCollection services, IEnumerable<Assembly> registrationAssemblies)
        {
            var assemblies = registrationAssemblies.ToArray();
            foreach (var assembly in assemblies)
            {
                services.AddValidatorsFromAssembly(assembly);
            }
            return services;
        }
        public static IServiceCollection RegisterDomainEvents(this IServiceCollection services, IEnumerable<Assembly> registrationAssemblies)
        {
            var assemblies = registrationAssemblies.ToArray();
            foreach (var assembly in assemblies)
            {
                services.AddValidatorsFromAssembly(assembly);
            }
            return services;
        }
        public static IServiceCollection RegisterMediatr(this IServiceCollection services, IEnumerable<Assembly> registrationAssemblies)
        {
            var assemblies = registrationAssemblies.ToArray();
            services.AddMediatR(configuration =>
            {

                foreach (var assembly in assemblies)
                {
                    configuration.RegisterServicesFromAssembly(assembly);
                }

                configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
                configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
                configuration.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
                configuration.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));
                configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
            });
            return services;
        }
        public static IServiceCollection RegisterDomainEventDispatcher(this IServiceCollection services)
        {

            services.AddScoped<IDispatchDomainEventsManager, DispatchDomainEventsInterceptor>();
            return services;
        }
    public static IServiceCollection RegisterServicesAutomatically(this IServiceCollection services, IApiOptions options,
      IConfiguration configuration)
    {
        var proxyGenerator = new ProxyGenerator();

        foreach (var assembly in options.RegistrationAssemblies)
        {
            var serviceTypes = ApiInterceptedServicesCollectionExtensions.GetServiceTypesFromAssembly(assembly);

            foreach (var serviceType in serviceTypes)
            {
                ApiInterceptedServicesCollectionExtensions.RegisterService(services, serviceType, proxyGenerator, options, configuration);
            }
        }
        return services;
    }
}
