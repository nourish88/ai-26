using Juga.Abstractions.Data.AuditLog;
using Juga.DataAudit.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
namespace Juga.DataAudit.PostreSql.Configurations;

public static class DataAuditPostgreSqlServiceCollectionExtensions
{
    /// <summary>
    /// Data Audit Log PostgreSql konfigurasyonunun kaydı için kullanılır.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureDataAuditPostgreSql(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DataAuditPostgreSqlOptions>(configuration.GetSection(DataAuditPostgreSqlOptions.DataAuditPostgreSqlOptionsSection));
        services.AddScoped<IAuditLogStore, AuditLogStorePostgreSql>();
        services.AddScoped<IAuditEventCreator, AuditEventCreator>();
        return services;
    }
    /// <summary>
    /// Data Audit Log PostgreSql konfigurasyonunun kaydı için kullanılır.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureDataAuditPostgreSql(this IServiceCollection services, Action<DataAuditPostgreSqlOptions> action)
    {
        services.Configure<DataAuditPostgreSqlOptions>(action);
        services.AddScoped<IAuditLogStore, AuditLogStorePostgreSql>();
        services.AddScoped<IAuditEventCreator, AuditEventCreator>();
        return services;
    }
}