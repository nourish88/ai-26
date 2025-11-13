using AdminBackend.Application.Services.DataManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdminBackend.Infrastructure.Services.DataManager;

public static class DataManagerServiceExtensions
{
    public static IServiceCollection AddDataManagerService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient<IDataManagerService, DataManagerService>(options =>
        {
            options.BaseAddress = new Uri(configuration.GetValue<string>("DataManager:Url")!);
        });
        
        return services;
    }
}