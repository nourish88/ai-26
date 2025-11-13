namespace Juga.Data.AuditLogging;

internal class NullAuditLogStore : IAuditLogStore
{
    public void StoreAuditEvents(IEnumerable<AuditEvent> auditEventsFucn)
    {

    }
}