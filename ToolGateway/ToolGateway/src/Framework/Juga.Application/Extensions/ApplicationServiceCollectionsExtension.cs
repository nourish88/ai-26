using System.Reflection;
using Juga.Application.Pipelines.Caching;
using Juga.Application.Pipelines.RequestResponse;
using Juga.Application.Pipelines.Transaction;
using Juga.Application.Pipelines.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Juga.Application.Extensions;

public static class ApplicationServiceCollectionsExtension
{
    public static IServiceCollection TryAddApplicationServices(this IServiceCollection services, IEnumerable<Assembly> registrationAssemblies)
    {
        var assemblies = registrationAssemblies.ToArray();
        foreach (var assembly in assemblies)
        {
            services.AddValidatorsFromAssembly(assembly);
        }
       
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

}