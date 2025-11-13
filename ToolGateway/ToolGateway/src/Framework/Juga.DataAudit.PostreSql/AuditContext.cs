using Juga.DataAudit.Common;
using Microsoft.EntityFrameworkCore;

namespace Juga.DataAudit.PostreSql;

internal class AuditContext(string connectionString) : DbContext
{
    public DbSet<InternalEntity> DataAuditEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString);
    }
}