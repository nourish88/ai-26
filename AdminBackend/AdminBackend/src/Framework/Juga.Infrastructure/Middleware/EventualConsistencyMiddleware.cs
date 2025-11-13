using Juga.Data.Abstractions;
using Juga.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Juga.Infrastructure.Middleware;

public class EventualConsistencyMiddleware(RequestDelegate next, IUnitOfWork baseDbContext)
{
    public const string DomainEventsKey = "DomainEventsKey";
    //TODO: IPublisher yerine Masstransit düşünülebilir mi?
    public async Task InvokeAsync(HttpContext context, IPublisher publisher)
    {
        var transaction = await baseDbContext.GetDataBase().BeginTransactionAsync();
        context.Response.OnCompleted(async () =>
        {
            try
            {
                if (context.Items.TryGetValue(DomainEventsKey, out var value) && value is Queue<IDomainEvent> domainEvents)
                {
                    while (domainEvents.TryDequeue(out var nextEvent))
                    {
                        await publisher.Publish(nextEvent);
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        });

        await next(context);
    }
}