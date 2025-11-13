namespace Juga.Abstractions.Data.AuditLog;

/// <summary>
/// Audit logların otomatik oluşup oluşmayacağını belirmek için kullanılır.
/// </summary>
public enum AuditBehaviour
{
    /// <summary>
    /// Oluşturulsun
    /// </summary>
    Enabled,

    /// <summary>
    /// Oluşturulmasın
    /// </summary>
    Disabled
}