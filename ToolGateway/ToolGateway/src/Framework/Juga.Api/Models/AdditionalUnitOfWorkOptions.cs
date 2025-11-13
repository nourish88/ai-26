using Juga.Api.Enums;

namespace Juga.Api.Models;

public record AdditionalUnitOfWorkOption <TContext> where TContext : UnitOfWork
{
    public EfCoreDbProviders Provider { get; set; }
    public UnitOfWork Ctx { get; set; }
    public string ConnectionString { get; set; } = default!;
}

