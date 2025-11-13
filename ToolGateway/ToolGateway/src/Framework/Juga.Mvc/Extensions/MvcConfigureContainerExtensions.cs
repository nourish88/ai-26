using Autofac;
using Juga.Client.Abstractions;
using Juga.Mvc.Helpers;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Juga.Mvc.Extensions;

public static class MvcConfigureContainerExtensions
{
    public static void ConfigureContainer(this ContainerBuilder builder, MvcOptions options)
    {
        builder.RegisterAssemblyTypes(options.RegistrationAssemblies.ToArray()).
            Where(t => t.Name.EndsWith("Service")).AsSelf();

        if (options.ApiClientList != null)
            foreach (var apiClient in options.ApiClientList)
            {
                var implementedInterfaces = apiClient.GetInterfaces();
                if (!implementedInterfaces.Any())
                    builder.Register(scope =>
                    {
                        return GetClientInstance(scope, apiClient);
                    }).As(apiClient);
                else
                    builder.Register(scope =>
                    {
                        return GetClientInstance(scope, apiClient);
                    }).As(implementedInterfaces.FirstOrDefault());
            }

    }
    private static object GetClientInstance(IComponentContext scope, Type apiClient)
    {
        //Explicitly ensuring the ctor function above is called, and also showcasing why this is an anti-pattern.
        var httpClientFactory = scope.Resolve<IHttpClientFactory>();
        var tokenProvider = scope.Resolve<ITokenProvider>();

        var accessToken = tokenProvider.GetToken(TokenType.AccessToken).Result;
        var client = httpClientFactory.CreateClient(Regex.Replace(apiClient.Name, "Client$", ""));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //TODO: Clean up both the IServiceINeedToUse and IOtherService configuration here, then somehow rebuild the service tree.
        //Wow!
        if (apiClient.GetConstructors().All(c => c.GetParameters().Length == 2))
            return Activator.CreateInstance(apiClient, new object[] { null, client });
        else
            return Activator.CreateInstance(apiClient, new object[] { client });
    }
}