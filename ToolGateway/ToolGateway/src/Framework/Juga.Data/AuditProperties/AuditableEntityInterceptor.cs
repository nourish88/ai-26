namespace Juga.Data.AuditProperties;

public class AuditableEntityInterceptor(IUserContextProvider userContextProvider,
    IAuditPropertyInterceptorManager auditPropertyInterceptorManager)
    : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    private void UpdateAuditableEntities(DbContext context)
    {

        var entities = context.ChangeTracker.Entries().ToList();

        foreach (var entity in entities)
        {
            if ((entity.State == EntityState.Added || entity.State == EntityState.Modified || entity.State == EntityState.Deleted)
                && auditPropertyInterceptorManager != null && userContextProvider != null)
            {
                auditPropertyInterceptorManager.OnSave(userContextProvider, entity);
            }
        }
    }
}