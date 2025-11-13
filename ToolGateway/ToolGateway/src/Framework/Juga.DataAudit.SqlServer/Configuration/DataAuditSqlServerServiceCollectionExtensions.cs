using Juga.Abstractions.Data.AuditLog;
using Juga.DataAudit.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Juga.DataAudit.SqlServer.Configuration;

public static class DataAuditSqlServerServiceCollectionExtensions
{
    /// <summary>
    /// Data Audit Log SqlServer konfigurasyonunun kaydı için kullanılır.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureDataAuditSqlServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DataAuditSqlServerOptions>(configuration.GetSection(DataAuditSqlServerOptions.DataAuditSqlServerOptionsSection));
        services.AddScoped<IAuditLogStore, AuditLogStoreSqlServer>();
        services.AddScoped<IAuditEventCreator, AuditEventCreator>();
        return services;
    }

    /// <summary>
    /// Data Audit Log SqlServer konfigurasyonunun kaydı için kullanılır.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureDataAuditSqlServer(this IServiceCollection services, Action<DataAuditSqlServerOptions> action)
    {
        services.Configure<DataAuditSqlServerOptions>(action);
        services.AddScoped<IAuditLogStore, AuditLogStoreSqlServer>();
        services.AddScoped<IAuditEventCreator, AuditEventCreator>();
        return services;
    }
}