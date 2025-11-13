using Juga.Data.Abstractions;
using Juga.Data.Repository;

namespace Juga.Api.Extensions;

public static class ApiRepositoryAutoRegistrationServiceCollectionExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IApiOptions options)
    {
        services.AddScoped(typeof(IRepository<>), typeof(DisconnectedRepository<>));

        var types = options.RegistrationAssemblies.SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Repository"))
            .ToList();
        foreach (var implementationType in types)
        {
            var interfaceType = options.RegistrationAssemblies.SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(i => i.Name == "I" + implementationType.Name);

            if (interfaceType != null) services.AddScoped(interfaceType, implementationType);
        }

        return services;
    }
}