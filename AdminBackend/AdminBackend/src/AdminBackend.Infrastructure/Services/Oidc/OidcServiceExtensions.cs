using AdminBackend.Application.Services.Oidc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdminBackend.Infrastructure.Services.Oidc;

public static class OidcServiceExtensions
{
    public static IServiceCollection AddOidcService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IOidcService, KeycloakOidcClient>();
        
        return services;
    }
}