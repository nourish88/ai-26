using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Juga.Api.Extensions;

public static class ApiClientsServiceCollectionExtension
{
    public static IServiceCollection AddApiClients(this IServiceCollection services, IApiOptions options)
    {
        if (options.ApiClientList is null) return services;
        foreach (var apiClient in options.ApiClientList)
        {
            var implementedInterfaces = apiClient.GetInterfaces();
            if (!implementedInterfaces.Any())
                services.AddScoped(apiClient, provider => GetClientInstance(provider, apiClient));
            else
                services.AddScoped(implementedInterfaces.First(), provider => GetClientInstance(provider, apiClient));
        }

        return services;
    }

    private static object GetClientInstance(IServiceProvider provider, Type apiClient)
    {
        var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
        var accessToken = provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Request
            .Headers["Authorization"].ToString().Replace("Bearer ", "");
        var client = httpClientFactory.CreateClient(Regex.Replace(apiClient.Name, "Client$", ""));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        if (apiClient.GetConstructors().All(c => c.GetParameters().Length == 2))
            return Activator.CreateInstance(apiClient, null, client) ?? throw new InvalidOperationException();

        return Activator.CreateInstance(apiClient, client) ?? throw new InvalidOperationException();
    }
}