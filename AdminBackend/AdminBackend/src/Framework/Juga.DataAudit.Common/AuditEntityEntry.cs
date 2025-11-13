using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Juga.DataAudit.Common;

public class AuditEntityEntry
{
    public EntityEntry EntityEntry { get; set; }
    public string EventType { get; set; }
}