using Juga.Abstractions.Data.AuditLog;
using Juga.DataAudit.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Juga.DataAudit.Elastic.Configuration;

public static class DataAuditElasticServiceCollectionExtensions
{
    /// <summary>
    /// Data Audit Log ElasticSearch konfigurasyonunun kaydı için kullanılır.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureDataAuditElastic(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DataAuditElasticOptions>(configuration.GetSection(DataAuditElasticOptions.DataAuditElasticOptionsSection));
        services.AddSingleton<IElasticClientProvider, ElasticClientProvider>();
        services.AddScoped<IAuditLogStore, AuditLogStoreElastic>();
        services.AddScoped<IAuditEventCreator, AuditEventCreator>();
        return services;
    }
}