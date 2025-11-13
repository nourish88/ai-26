namespace Juga.Data.AuditProperties;

/// <summary>
/// UpdatedBy özelliği olup olmadığını kontrol eden interceptor.
/// Eğer IHasUpdatedBy varsa araya girip alanın oluşmasını sağlar ve değerini atar.
/// </summary>
public class HasUpdatedByInterceptor(IOptions<UnitOfWorkOptions> options) : IAuditPropertyInterceptor
{
    private readonly UnitOfWorkOptions _options = options.Value;
    public string PropertyName => "UpdatedBy";
    public bool Enabled => _options.EnableUpdatedByAuditField;

    public bool ShouldIntercept(Type type)
    {
        return type.GetInterfaces().Contains(typeof(IHasUpdatedBy));
    }
    public void OnModelCreating(EntityTypeBuilder entityTypeBuilder)
    {
        var property = entityTypeBuilder.Metadata.GetProperties().FirstOrDefault(p => p.Name == PropertyName);
        if (property == null)
        {
            entityTypeBuilder.Property<string>(PropertyName).HasMaxLength(64);
        }
    }
    public void OnInsert(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        return;
    }
    public void OnUpdate(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        entityEntry.Property(PropertyName).CurrentValue = clientInfoProvider.ClientId;
    }

    public void OnDelete(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        return;
    }
}