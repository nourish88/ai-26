
using Juga.Domain.Interfaces;

namespace Juga.Domain.Base;

public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IDomainEvent[] ClearDomainEvents()
    {
        var dequeedDomainEvents = _domainEvents.ToArray();
        _domainEvents.Clear();
        return dequeedDomainEvents;
    }



}

