using Juga.Abstractions.Client;
using Juga.Abstractions.Data.AuditLog;
using Microsoft.EntityFrameworkCore;
namespace Juga.DataAudit.Common;

public interface IAuditEventCreator
{
    Func<IEnumerable<AuditEvent>> CreateAuditEvents(DbContext dbContext, IUserContextProvider clientInfoProvider, DateTime eventTime);
    void SetChanges(DbContext dbContext);
    IEnumerable<AuditEvent> GetEvents(DbContext dbContext, IUserContextProvider clientInfoProvider, DateTime eventTime);

}