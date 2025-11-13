namespace Juga.Abstractions.Data.AuditLog;

/// <summary>
/// Audit loglarının kaydedilmesi için implementasyonu yapılacak interface
/// </summary>
public interface IAuditLogStore
{
    // <summary>
    /// Data audit eventlerinin kaydedilmesi için kullanılır.
    /// </summary>
    /// <param name="auditEvents">Data Audit Eventleri</param>
    void StoreAuditEvents(IEnumerable<AuditEvent> auditEvents);
}