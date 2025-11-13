namespace Juga.Data.AuditProperties;

/// <summary>
/// CreatedDate özelliği olup olmadığını kontrol eden interceptor.
/// Eğer IHasCreateDate varsa araya girip alanın oluşmasını sağlar ve değerini atar.
/// </summary>
public class HasCreateDateInterceptor(IOptions<UnitOfWorkOptions> options) : IAuditPropertyInterceptor
{
    private readonly UnitOfWorkOptions _options = options.Value;
    public string PropertyName => "CreatedDate";


    public bool Enabled => _options.EnableCreatedDateAuditField;

    public bool ShouldIntercept(Type type)
    {
        return type.GetInterfaces().Contains(typeof(IHasCreateDate));
    }
    public void OnModelCreating(EntityTypeBuilder entityTypeBuilder)
    {
        var property = entityTypeBuilder.Metadata.GetProperties().FirstOrDefault(p => p.Name == PropertyName);
        if (property == null)
        {
            entityTypeBuilder.Property<DateTime>(PropertyName).IsRequired();
        }
    }


    public void OnInsert(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
        entityEntry.Property(PropertyName).CurrentValue = _options.DatabaseOptions.DatabaseType switch
        {
            Juga.Abstractions.Data.Enums.DatabaseType.SqlServer => DateTime.Now,
            Juga.Abstractions.Data.Enums.DatabaseType.PostgreSql => DateTime.UtcNow,
            _ => throw new ArgumentException(
                $"{nameof(_options.DatabaseOptions.DatabaseType)} is not supported for {nameof(HasCreateDateInterceptor)}")
        };
    }

    public void OnUpdate(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
    }

    public void OnDelete(IUserContextProvider clientInfoProvider, EntityEntry entityEntry)
    {
    }
}