namespace Juga.Data.AuditLogging;

internal class NullAuditEventCreator : IAuditEventCreator
{
    public Func<IEnumerable<AuditEvent>> CreateAuditEvents(DbContext dbContext, IUserContextProvider clientInfoProvider, DateTime eventTime)
    {
        return () => new List<AuditEvent>();
    }

    public IEnumerable<AuditEvent> GetEvents(DbContext dbContext, IUserContextProvider clientInfoProvider, DateTime eventTime)
    {
        return new List<AuditEvent>();
    }

    public void SetChanges(DbContext dbContext)
    {

    }
}