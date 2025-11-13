namespace Juga.Data.AuditProperties;

/// <summary>
/// CreatedAt özelliği olup olmadığını kontrol eden interceptor.
/// Eğer IHasCreatedAt varsa araya girip alanın oluşmasını sağlar ve değerini atar.
/// </summary>
public class HasCreatedAtInterceptor(IOptions<UnitOfWorkOptions> options) : IAuditPropertyInterceptor
{
    private readonly UnitOfWorkOptions _options = options.Value;
    public string PropertyName => "CreatedAt";
    public bool Enabled => _options.EnableCreatedAtAuditField;

    public bool ShouldIntercept(Type type)
    {
        return type.GetInterfaces().Contains(typeof(IHasCreatedAt));
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
        entityEntry.Property(PropertyName).CurrentValue = clientInfoProvider.ClientIp;
    }

    public void OnUpdate(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        return;
    }

    public void OnDelete(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        return;
    }
}