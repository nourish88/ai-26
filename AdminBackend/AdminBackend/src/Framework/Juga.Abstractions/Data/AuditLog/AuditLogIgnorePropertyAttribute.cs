namespace Juga.Abstractions.Data.AuditLog;

/// <summary>
/// Bir entity property sinde yapılan değişikliklerin audit log kayıtlarına yansıtılmaması için kullanılır.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class AuditLogIgnorePropertyAttribute : Attribute
{
}