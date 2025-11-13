namespace Juga.Abstractions.Data.AuditLog;

/// <summary>
/// Entity üst verisi için kullanılır
/// </summary>
public class EntityMetaData
{
    /// <summary>
    /// Entitynin database ismini ifade eder
    /// </summary>
    public string DatabaseName { get; set; }

    /// <summary>
    /// Entitynin schema ismini ifade eder
    /// </summary>
    public string SchemaName { get; set; }

    /// <summary>
    /// Entitiynin tablo ismini ifade eder
    /// </summary>
    public string TableName { get; set; }
}