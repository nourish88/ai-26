namespace Juga.Data.AuditProperties;

public class AuditPropertyInterceptorManager : IAuditPropertyInterceptorManager
{
    private readonly HashSet<IAuditPropertyInterceptor> _interceptors;
    public AuditPropertyInterceptorManager(IEnumerable<IAuditPropertyInterceptor> auditPropertyInterceptors)
    {
        _interceptors = new HashSet<IAuditPropertyInterceptor>();
        foreach (var interceptor in auditPropertyInterceptors)
        {
            if (interceptor.Enabled)
            {
                _interceptors.Add(interceptor);
            }
        }
    }

    public void OnModelCreating(EntityTypeBuilder entityTypeBuilder)
    {
        foreach (var interceptor in _interceptors)
        {
            if (interceptor.ShouldIntercept(entityTypeBuilder.Metadata.ClrType))
            {
                interceptor.OnModelCreating(entityTypeBuilder);
            }
        }
    }

    public void OnSave(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {

        switch (entityEntry.State)
        {
            case EntityState.Added:
                {
                    OnInsert(clientInfoProvider, entityEntry);
                    break;
                }
            case EntityState.Modified:
                {
                    OnUpdate(clientInfoProvider, entityEntry);
                    break;
                }
            case EntityState.Deleted:
                {
                    OnDelete(clientInfoProvider, entityEntry);
                    break;
                }
            default:
                {
                    return;
                }
        }
    }

    private void OnInsert(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        foreach (var interceptor in _interceptors)
        {
            if (interceptor.ShouldIntercept(entityEntry.Metadata.ClrType))
            {
                interceptor.OnInsert(clientInfoProvider, entityEntry);
            }
        }
    }

    private void OnUpdate(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        foreach (var interceptor in _interceptors)
        {
            if (interceptor.ShouldIntercept(entityEntry.Metadata.ClrType))
            {
                interceptor.OnUpdate(clientInfoProvider, entityEntry);
            }
        }
    }

    private void OnDelete(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        foreach (var interceptor in _interceptors)
        {
            if (interceptor.ShouldIntercept(entityEntry.Metadata.ClrType))
            {
                interceptor.OnDelete(clientInfoProvider, entityEntry);
            }
        }
    }
}