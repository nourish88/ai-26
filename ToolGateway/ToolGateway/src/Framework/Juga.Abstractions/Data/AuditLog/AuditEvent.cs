namespace Juga.Abstractions.Data.AuditLog;

/// <summary>
/// Audit tablosuna yazılacak alanlar buradan tanımlanır
/// </summary>
public class AuditEvent
{
    /// <summary>
    /// Varsa tablo primary key 1
    /// </summary>
    public long? PkValue1 { get; set; }

    /// <summary>
    /// Varsa tablo primary key 2
    /// </summary>
    public long? PkValue2 { get; set; }

    public Guid? PkGuid { get; set; }
    public string DatabaseName { get; set; }
    public string SchemaName { get; set; }
    public string TableName { get; set; }
    public string EventType { get; set; }
    public Dictionary<string, object> PropertyValues { get; set; }
    public DateTime EventTime { get; set; }
    public string User { get; set; }
}