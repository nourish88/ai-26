namespace Juga.Data.AuditProperties;

public interface IAuditPropertyInterceptor
{
    bool Enabled { get; }
    string PropertyName { get; }
    bool ShouldIntercept(Type type);
    void OnModelCreating(EntityTypeBuilder entityTypeBuilder);
    void OnInsert(IUserContextProvider clientInfoProvider, EntityEntry entityEntry);
    void OnDelete(IUserContextProvider clientInfoProvider, EntityEntry entityEntry);
    void OnUpdate(IUserContextProvider clientInfoProvider, EntityEntry entityEntry);
}