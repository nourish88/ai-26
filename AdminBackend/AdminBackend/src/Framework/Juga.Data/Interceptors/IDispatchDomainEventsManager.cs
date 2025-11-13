
namespace Juga.Data.Interceptors;

public interface IDispatchDomainEventsManager : ISaveChangesInterceptor
{
    Task DispatchDomainEvent(DbContext? context);
}

