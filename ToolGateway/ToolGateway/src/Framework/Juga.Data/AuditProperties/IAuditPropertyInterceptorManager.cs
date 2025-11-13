namespace Juga.Data.AuditProperties;
public interface IAuditPropertyInterceptorManager
{
    void OnModelCreating(EntityTypeBuilder entityTypeBuilder);
    void OnSave(IUserContextProvider clientInfoProvider, EntityEntry entityEntry);
}