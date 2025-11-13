using Juga.Api.Enums;

namespace Juga.Api.Models;

public sealed record WebApiStartUpConfig(
    WebApplicationBuilder? Builder,
    string ConnectionStringName,
    EfCoreDbProviders? BaseDbProvider = EfCoreDbProviders.MsSql,
    AuditLogStoreType AuditLogStoreType = AuditLogStoreType.SqlServer,
    bool IsMinimal = false,
    string DbContextProjectName = "",
    string Redis = "Redis",
    string CachingMechanismSection = "Juga:Caching:GeneralSettings:Mechanism",
    string RedisEndpointNameSection = "Juga:Caching:GeneralSettings:RedisEndPoint",
    string OpenApiNameSection = "Juga:OpenApi:Name",
    List<UnitOfWorkConfig>? AdditionalUnitOfWorkConfigs = null,
    string MigrationAssemblyName=""


);
public class UnitOfWorkConfig
{
    public string ConnectionStringName { get; set; }
    public EfCoreDbProviders DbProvider { get; set; }

}