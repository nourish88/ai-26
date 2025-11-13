namespace Juga.Data.Interceptors;
public class DispatchDomainEventsInterceptor (IMediator mediator):SaveChangesInterceptor, IDispatchDomainEventsManager
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        DispatchDomainEvent(eventData.Context).GetAwaiter().GetResult();
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new ())
    {
        await DispatchDomainEvent(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public virtual async Task DispatchDomainEvent(DbContext? context)
    {
        if (context == null) return;
        var aggregates = context.ChangeTracker.Entries<IAggregate>().Where(a => a.Entity.DomainEvents.Any())
            .Select(q => q.Entity);

        var domainEvents = aggregates.SelectMany(a => a.DomainEvents).ToList();
       
        aggregates.ToList().ForEach(q=>q.ClearDomainEvents());
        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}


