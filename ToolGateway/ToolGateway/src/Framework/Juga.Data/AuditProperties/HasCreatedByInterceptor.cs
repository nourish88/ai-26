namespace Juga.Data.AuditProperties;

/// <summary>
/// CreatedBy özelliği olup olmadığını kontrol eden interceptor.
/// Eğer IHasCreatedBy varsa araya girip alanın oluşmasını sağlar ve değerini atar.
/// </summary>
public class HasCreatedByInterceptor(IOptions<UnitOfWorkOptions> options) : IAuditPropertyInterceptor
{
    private readonly UnitOfWorkOptions _options = options.Value;
    public string PropertyName => "CreatedBy";
    public bool Enabled => _options.EnableCreatedByAuditField;

    public bool ShouldIntercept(Type type)
    {
        return type.GetInterfaces().Contains(typeof(IHasCreatedBy));
    }

    public void OnModelCreating(EntityTypeBuilder entityTypeBuilder)
    {
        var property = entityTypeBuilder.Metadata.GetProperties().FirstOrDefault(p => p.Name == PropertyName);
        if (property == null)
        {
            entityTypeBuilder.Property<string>(PropertyName).IsRequired().HasMaxLength(64);
        }
    }

    public void OnInsert(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        entityEntry.Property(PropertyName).CurrentValue = clientInfoProvider.ClientId;
    }

    public void OnUpdate(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
    }

    public void OnDelete(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
    }
}