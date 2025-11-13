namespace Juga.Data.AuditProperties;

/// <summary>
/// UpdateDate özelliği olup olmadığını kontrol eden interceptor.
/// Eğer IHasUpdateDate varsa araya girip alanın oluşmasını sağlar ve değerini atar.
/// </summary>
public class HasUpdateDateInterceptor(IOptions<UnitOfWorkOptions> options) : IAuditPropertyInterceptor
{
    private readonly UnitOfWorkOptions _options = options.Value;
    public string PropertyName => "UpdatedDate";
    public bool Enabled => _options.EnableUpdatedDateAuditField;

    public bool ShouldIntercept(Type type)
    {
        var a = type.GetInterfaces().Contains(typeof(IHasUpdateDate));
        return a;
    }
    public void OnModelCreating(EntityTypeBuilder entityTypeBuilder)
    {
        var property = entityTypeBuilder.Metadata.GetProperties().FirstOrDefault(p => p.Name == PropertyName);
        if (property == null)
        {
            entityTypeBuilder.Property<DateTime?>(PropertyName);

        }
    }

    public void OnInsert(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        return;
    }

    public void OnUpdate(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        entityEntry.Property(PropertyName).CurrentValue = _options.DatabaseOptions.DatabaseType switch
        {
            Juga.Abstractions.Data.Enums.DatabaseType.SqlServer => DateTime.Now,
            Juga.Abstractions.Data.Enums.DatabaseType.PostgreSql => DateTime.UtcNow,
            _ => throw new ArgumentException(
                $"{nameof(_options.DatabaseOptions.DatabaseType)} is not supported for {nameof(HasCreateDateInterceptor)}")
        };
    }

    public void OnDelete(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        return;
    }
}