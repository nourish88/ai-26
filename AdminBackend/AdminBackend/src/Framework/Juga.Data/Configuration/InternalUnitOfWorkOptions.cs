namespace Juga.Data.Configuration;

/// <summary>
/// Migration ve EF Power Tools tarafından kullanılmak üzere oluşturulmuştur.
/// </summary>
internal class InternalUnitOfWorkOptions : UnitOfWorkOptions
{
    internal InternalUnitOfWorkOptions(DatabaseType databaseType)
    {
        base.DefaultDeleteBehavior = Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict;
        base.EnableCreatedAtAuditField = true;
        base.EnableCreatedByAuditField = true;
        base.EnableCreatedDateAuditField = true;
        base.EnableDatabaseCreation = false;
        base.EnableDatabaseDeletion = false;
        base.EnableDebuggerLogger = false;
        base.EnableUpdatedAtAuditField = true;
        base.EnableUpdatedByAuditField = true;
        base.EnableUpdatedDateAuditField = true;
        base.RunAsDisconnected = true;
        base.EnableCreatedByUserCodeAuditField = true;
        base.EnableUpdatedByUserCodeAuditField = true;
        base.DatabaseOptions = new DatabaseOptions() { DatabaseType = databaseType };
    }
}