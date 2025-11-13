namespace Juga.Abstractions.Data.AuditLog;

/// <summary>
/// Bir entity de yapılan değişikliklerin audit log kayıtlarına yansıtılmaması için kullanılır.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class AuditLogIgnoreAttribute : Attribute
{
}