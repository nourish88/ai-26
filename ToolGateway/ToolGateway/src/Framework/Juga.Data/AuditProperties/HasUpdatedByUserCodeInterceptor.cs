namespace Juga.Data.AuditProperties;

public class HasUpdatedByUserCodeInterceptor(IOptions<UnitOfWorkOptions> options) : IAuditPropertyInterceptor
{
    private readonly UnitOfWorkOptions _options = options.Value;
    public string PropertyName => "UpdatedByUserCode";
    public bool Enabled => _options.EnableUpdatedByUserCodeAuditField;

    public bool ShouldIntercept(Type type)
    {
        return type.GetInterfaces().Contains(typeof(IHasUpdatedByUserCode));
    }
    public void OnModelCreating(EntityTypeBuilder entityTypeBuilder)
    {
        var property = entityTypeBuilder.Metadata.GetProperties().FirstOrDefault(p => p.Name == PropertyName);
        if (property == null)
        {
            entityTypeBuilder.Property<string>(PropertyName).HasMaxLength(64); ;
        }
    }
    public void OnInsert(IUserContextProvider userContextProvider, EntityEntry entityEntry)
    {
        return;
    }
    public void OnUpdate(IUserContextProvider userContextProvider, EntityEntry entityEntry)
    {
        entityEntry.Property(PropertyName).CurrentValue = userContextProvider.UserCode;
    }

    public void OnDelete(IUserContextProvider userContextProvider, EntityEntry entityEntry)
    {
        return;
    }
}