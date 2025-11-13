namespace Juga.Abstractions.Data.AuditLog;

/// <summary>
/// Audit tablosuna yazılacak satırın işlem tipi
/// </summary>
public class AuditEventType
{
    public const string Insert = "Insert";
    public const string Update = "Update";
    public const string Delete = "Delete";
    public const string Unknown = "Unknown";
}