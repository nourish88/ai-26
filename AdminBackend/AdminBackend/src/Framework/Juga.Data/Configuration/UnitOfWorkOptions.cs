
namespace Juga.Data.Configuration;

/// <summary>
/// UnitOfWork servisinin konfigurasyonu için kullanılır.
/// </summary>
public class UnitOfWorkOptions
{
    public const string UnitOfWorkOptionsSection = "Juga:Data:UnitOfWork";
    /// <summary>
    /// DbContext in connected disconnected state de çalışmasını belirlemek için kullanılır.
    /// </summary>
    /// <seealso cref="Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker"/>
    /// <remarks>ChangeTrackerı pasif hale getirir. Request Response modeli uygulamalarda disconnected çalışması performans açısından tavsiye edilir.</remarks>
    public virtual bool RunAsDisconnected { get; set; }

    /// <summary>
    /// Insert sırasında, CreatedDate alanının otomatik doldurulmasını aktif etmek için kullanılır.
    /// </summary>
    public virtual bool EnableCreatedDateAuditField { get; set; }


    /// <summary>
    /// Update sırasında UpdateddDate alanının otomatik doldurulmasını aktif etmek için kullanılır.
    /// </summary>
    public virtual bool EnableUpdatedDateAuditField { get; set; }

    /// <summary>
    /// Insert sırasında CreatedBy alanının otomatik doldurulmasını aktif etmek için kullanılır.
    /// </summary>
    public virtual bool EnableCreatedByAuditField { get; set; }

    /// <summary>
    /// Update sırasında UpdatedBy alanının otomatik doldurulmasını aktif etmek için kullanılır.
    /// </summary>
    public virtual bool EnableUpdatedByAuditField { get; set; }

    /// <summary>
    /// Insert sırasında CreatedAt alanının otomatik doldurulmasını aktif etmek için kullanılır.
    /// </summary>
    public virtual bool EnableCreatedAtAuditField { get; set; }

    /// <summary>
    /// Update sırasında UpdatedAt alanının otomatik doldurulmasını aktif etmek için kullanılır.
    /// </summary>
    public virtual bool EnableUpdatedAtAuditField { get; set; }

    /// <summary>
    /// Insert sırasında CreatedByUserCode alanının otomatik doldurulmasını aktif etmek için kullanılır.
    /// </summary>
    public virtual bool EnableCreatedByUserCodeAuditField { get; set; }

    /// <summary>
    /// Update sırasında CreatedByUserCode alanının otomatik doldurulmasını aktif etmek için kullanılır.
    /// </summary>
    public virtual bool EnableUpdatedByUserCodeAuditField { get; set; }

    /// <summary>
    /// EFCore loglarının debuggera yazdırılması için kullanılır.
    /// </summary>
    /// <remarks>Development haricinde kullanılması tavsiye edilmez.</remarks>
    public virtual bool EnableDebuggerLogger { get; set; }

    /// <summary>
    /// Database oluşturulup oluşturlamayacağını belirmek için kullanılır.
    /// </summary>
    /// <remarks>Test haricinde kullanılmamalıdır.</remarks>
    public virtual bool EnableDatabaseCreation { get; set; }
    /// <summary>
    /// Database in silinip silinmeyeceğini belirmek için kullanılır.
    /// </summary>
    /// <remarks>Test haricinde kullanılmamalıdır.</remarks>
    public virtual bool EnableDatabaseDeletion { get; set; }

    /// <summary>
    /// Silinen bir kayda bağlı olan kayıtlar varsa (FK ile) bu kayıtlar için ne yapılacağını belirtmek için kullanılır.
    /// </summary>
    public virtual DeleteBehavior DefaultDeleteBehavior { get; set; }

    /// <summary>
    /// Audit kayıtlarının oluşturulup oluşturulmayacağını belirlemek için kullanılır.
    /// </summary>
    public virtual bool EnableDataAudit { get; set; }

    /// <summary>
    /// Veri tabanı özelliklerinin belirlenmesi için kullanılır.
    /// </summary>
    public virtual DatabaseOptions DatabaseOptions { get; set; }

    /// <summary>
    /// Save Changes işleminin transaction scope a alınıp alınmayacağını belirtir. AuditLogların transaction içinde olması için eklendi.
    /// </summary>
    public virtual bool RunSaveChangesAsTransactional { get; set; }
}