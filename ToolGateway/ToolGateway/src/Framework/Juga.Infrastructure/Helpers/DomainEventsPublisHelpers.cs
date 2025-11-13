//using Juga.Data;
//using Juga.Domain.Base;
//using Juga.Domain.Interfaces;
//using Juga.Infrastructure.Middleware;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//namespace Juga.Infrastructure.Helpers;

//public class DomainEventsPublisHelpers(IHttpContextAccessor httpContextAccessor, IPublisher publisher, IConfiguration configuration) : UnitOfWork
//{
//    public List<IDomainEvent> GetDomainEvents()
//    {
//        //List<IDomainEvent> domainEvents = idType switch
//        //{
//        //    IdType.Guid => ChangeTracker.Entries<GuidEntity>()
//        //        .SelectMany(entry => entry.Entity.PopDomainEvents())
//        //        .ToList(),
//        //    IdType.Long => ChangeTracker.Entries<LongEntity>()
//        //        .SelectMany(entry => entry.Entity.PopDomainEvents())
//        //        .ToList(),
//        //    IdType.Int => ChangeTracker.Entries<IntEntity>()
//        //        .SelectMany(entry => entry.Entity.PopDomainEvents())
//        //        .ToList(),
//        //    _ => throw new ArgumentOutOfRangeException(nameof(idType), idType, null)
//        //};
//        var domainEvents
//            = new List<IDomainEvent>();

//        domainEvents.AddRange(ChangeTracker.Entries<GuidEntity>()
//            .SelectMany(entry => entry.Entity.PopDomainEvents())
//            .ToList());
//        domainEvents.AddRange(ChangeTracker.Entries<IntEntity>()
//            .SelectMany(entry => entry.Entity.PopDomainEvents())
//            .ToList());
//        domainEvents.AddRange(ChangeTracker.Entries<LongEntity>()
//            .SelectMany(entry => entry.Entity.PopDomainEvents())
//            .ToList());
//        return domainEvents;
//    }

//    public async Task PublishDomainEvents(List<IDomainEvent> domainEvents)
//    {
//        foreach (var domainEvent in domainEvents) await publisher.Publish(domainEvent);
//    }

//    public bool IsUserWaitingOnline()
//    {
//        return httpContextAccessor.HttpContext is not null;
//    }

//    public void AddDomainEventsToOfflineProcessingQueue(List<IDomainEvent> domainEvents)
//    {
//        var domainEventsQueue =
//            httpContextAccessor.HttpContext!.Items.TryGetValue(EventualConsistencyMiddleware.DomainEventsKey,
//                out var value) &&
//            value is Queue<IDomainEvent> existingDomainEvents
//                ? existingDomainEvents
//                : new Queue<IDomainEvent>();

//        domainEvents.ForEach(domainEventsQueue.Enqueue);
//        httpContextAccessor.HttpContext.Items[EventualConsistencyMiddleware.DomainEventsKey] = domainEventsQueue;
//    }
//}