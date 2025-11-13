using Asp.Versioning.Builder;
using ApiVersion = Asp.Versioning.ApiVersion;

namespace Juga.Api.Helpers;

public interface IApiOptions
{
    public bool IsMinimal { get; set; }
    public IEnumerable<Assembly> RegistrationAssemblies { get; set; }


    public bool? Mediator { get; set; }
    public string ApiName { get; set; }
    public bool? HealthCheck { get; set; }
    public IEnumerable<Type>? HubList { get; set; }

    public IEnumerable<Type>? ApiClientList { get; set; }
    public bool RegisterQueueServices { get; set; }
}
public class ModularApiOptions : IApiOptions
{
    public IEnumerable<Assembly> AsyncRegistrationAssemblies { get; set; }
    public IEnumerable<Type>? HubList { get; set; } = [];
    public bool IsMinimal { get; set; } = false;
    public IEnumerable<Assembly> RegistrationAssemblies { get; set; } = default!;
    public string? ApiName { get; set; }
    public bool? Mediator { get; set; } = false;
    public bool? HealthCheck { get; set; } = false;

    public IEnumerable<Type>? ApiClientList { get; set; }
    public bool RegisterQueueServices { get; set; } = true;

}
public class ApiOptions : ModularApiOptions
{
    /// <summary>
    ///     FluentValidation deklerasyonlarinin aranacagi assembly'ler
    /// </summary>

    public IEnumerable<Type>? HubList { get; set; }

    public bool PrimaryContext { get; set; } = true;
    public IEnumerable<Type>? ExternalServiceList { get; set; }


    public List<ApiVersion> MinimalApiVersion { get; set; } =
    [
        new ApiVersion(1, 0),
        new ApiVersion(2, 0)
    ];

    public ApiVersionSet? MinimalApiVersionSet { get; set; }

    ///// <summary>
    ///// AutoMapper Profilleri için
    ///// </summary>
    //public IEnumerable<Profile> MappingProfiles { get; set; }

    /// <summary>
    ///     Audit Log Store Tipini belilemek için kullanılır.Default SqlServer.
    /// </summary>
    public AuditLogStoreType AuditLogStoreType { get; set; } = AuditLogStoreType.SqlServer;
}