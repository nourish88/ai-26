namespace Juga.Data.AuditProperties;

public class HasCreatedByUserCodeInterceptor(IOptions<UnitOfWorkOptions> options) : IAuditPropertyInterceptor
{
    private readonly UnitOfWorkOptions _options = options.Value;
    public string PropertyName => "CreatedByUserCode";
    public bool Enabled => _options.EnableCreatedByUserCodeAuditField;

    public bool ShouldIntercept(Type type)
    {
        return type.GetInterfaces().Contains(typeof(IHasCreatedByUserCode));
    }

    public void OnModelCreating(EntityTypeBuilder entityTypeBuilder)
    {
        var property = entityTypeBuilder.Metadata.GetProperties().FirstOrDefault(p => p.Name == PropertyName);
        if (property == null)
        {
            entityTypeBuilder.Property<string>(PropertyName).HasMaxLength(64);
        }
    }

    public void OnInsert(IUserContextProvider userContextInfoProvider, EntityEntry entityEntry)
    {
        entityEntry.Property(PropertyName).CurrentValue = userContextInfoProvider.UserCode;
    }

    public void OnUpdate(IUserContextProvider userContextInfoProvider, EntityEntry entityEntry)
    {
        return;
    }

    public void OnDelete(IUserContextProvider userContextInfoProvider, EntityEntry entityEntry)
    {
        return;
    }
}