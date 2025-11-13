namespace Juga.DataAudit.SqlServer.Configuration;

/// <summary>
/// Data Audit SqlServer servisinin konfigurasyonu için kullanılır.
/// </summary>
public class DataAuditSqlServerOptions
{
    /// <summary>
    /// DataAuditSqlServerOptions ın konfigurasyonda bulunduğu section bilgisi. 
    /// </summary>
    public const string DataAuditSqlServerOptionsSection = "Juga:DataAudit:SqlServer";
    /// <summary>
    /// Data Auidt SqlServer için connection string bilgisi.
    /// </summary>
    public virtual string ConnectionString { get; set; }

    /// <summary>
    /// Audit kayıtlarının saklanacağı tablonun şema bilgisi. Default dbo.
    /// </summary>
    public virtual string SchemaName { get; set; } = "dbo";

    /// <summary>
    /// Audit kayıtlarının saklanacağı tablo ismi bilgisi. Default AuditLogs.
    /// </summary>
    public virtual string TableName { get; set; } = "AuditLogs";

    /// <summary>
    /// Audit kayıtlarının saklanacağı tablonun primary key stununun ismi. Default Id.
    /// </summary>
    public virtual string AuditTablePrimaryKeyColumnName { get; set; } = "Id";

    /// <summary>
    /// Audit kaydında bulunan Pk1 (Primary Key 1) <seealso cref="Abstractions.Data.AuditLog.AuditEvent.PkValue1"/> alanının, Audit tablosunda saklanacağı stunun ismi. 
    /// Default Pk1.
    /// </summary>
    public virtual string AuditTableColumnNameForPk1 { get; set; } = "Pk1";

    /// <summary>
    /// Audit kaydında bulunan Pk2 (Primary Key 2) <seealso cref="Abstractions.Data.AuditLog.AuditEvent.PkValue2"/> alanının, Audit tablosunda saklanacağı stunun ismi.
    /// Default Pk2.
    /// </summary>
    public virtual string AuditTableColumnNameForPk2 { get; set; } = "Pk2";

    /// <summary>
    /// Audit kaydında bulunan Pk2 (Primary Key 2) <seealso cref="Abstractions.Data.AuditLog.AuditEvent.PkGuid"/> alanının, Audit tablosunda saklanacağı stunun ismi.
    /// Default Pk2.
    /// </summary>
    public virtual string AuditTableColumnNameForPkGuid { get; set; } = "PkGuid";

    /// <summary>
    /// Audit kaydında bulunan DatabaseName <seealso cref="Abstractions.Data.AuditLog.AuditEvent.DatabaseName"/> alanının, Audit tablosunda saklanacağı stunun ismi.
    /// Default Database.
    /// </summary>
    public virtual string AuditTableColumnNameForDatabaseName { get; set; } = "Database";

    /// <summary>
    /// Audit kaydında bulunan SchemaName <seealso cref="Abstractions.Data.AuditLog.AuditEvent.SchemaName"/> alanının, Audit tablosunda saklanacağı stunun ismi.
    /// Default Schema.
    /// </summary>
    public virtual string AuditTableColumnNameForSchemaName { get; set; } = "Schema";

    /// <summary>
    /// Audit kaydında bulunan TableName <seealso cref="Abstractions.Data.AuditLog.AuditEvent.TableName"/> alanının, Audit tablosunda saklanacağı stunun ismi.
    /// Default Table.
    /// </summary>
    public virtual string AuditTableColumnNameForTableName { get; set; } = "Table";

    /// <summary>
    /// Audit kaydında bulunan EventType <seealso cref="Abstractions.Data.AuditLog.AuditEvent.EventType"/> alanının, Audit tablosunda saklanacağı stunun ismi.
    /// Default EventType.
    /// </summary>
    public virtual string AuditTableColumnNameForEventType { get; set; } = "EventType";

    /// <summary>
    /// Audit kaydında bulunan PropertyValues <seealso cref="Abstractions.Data.AuditLog.AuditEvent.PropertyValues"/> alanının, Audit tablosunda saklanacağı stunun ismi.
    /// Default Data.
    /// </summary>
    public virtual string AuditTableColumnNameForPropertyValues { get; set; } = "Data";

    /// <summary>
    /// Audit kaydında bulunan EventTime <seealso cref="Abstractions.Data.AuditLog.AuditEvent.EventTime"/> alanının, Audit tablosunda saklanacağı stunun ismi.
    /// Default EventTime.
    /// </summary>
    public virtual string AuditTableColumnNameForEventTime { get; set; } = "EventTime";

    /// <summary>
    /// Audit kaydında bulunan User <seealso cref="Abstractions.Data.AuditLog.AuditEvent.User"/> alanının, Audit tablosunda saklanacağı stunun ismi.
    /// Default User.
    /// </summary>
    public virtual string AuditTableColumnNameForUser { get; set; } = "User";
}