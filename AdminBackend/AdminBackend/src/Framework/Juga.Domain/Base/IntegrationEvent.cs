
using Juga.Abstractions.Messaging;

namespace Juga.Domain.Base;

public record IntegrationEvent : IEvent
{
    Guid EventId => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName!;
}

