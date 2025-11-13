namespace Juga.Data.AuditProperties;

/// <summary>
/// Migration ve EF Power Tools tarafından kullanılmak üzere oluşturulmuştur.
/// </summary>
internal class InternalAuditPropertyInterceptorManager : AuditPropertyInterceptorManager
{
    public InternalAuditPropertyInterceptorManager(UnitOfWorkOptions unitOfWorkOptions) : base(new List<IAuditPropertyInterceptor>
    {
        new HasCreateDateInterceptor(new OptionsWrapper<UnitOfWorkOptions>(unitOfWorkOptions)),
        new HasCreatedAtInterceptor(new OptionsWrapper<UnitOfWorkOptions>(unitOfWorkOptions)),
        new HasCreatedByInterceptor(new OptionsWrapper<UnitOfWorkOptions>(unitOfWorkOptions)),
        new HasUpdateDateInterceptor(new OptionsWrapper<UnitOfWorkOptions>(unitOfWorkOptions)),
        new HasUpdatedByInterceptor(new OptionsWrapper<UnitOfWorkOptions>(unitOfWorkOptions)),
        new HasUpdatedAtInterceptor(new OptionsWrapper<UnitOfWorkOptions>(unitOfWorkOptions)),
        new HasCreatedByUserCodeInterceptor(new OptionsWrapper<UnitOfWorkOptions>(unitOfWorkOptions)),
        new HasUpdatedByUserCodeInterceptor(new OptionsWrapper<UnitOfWorkOptions>(unitOfWorkOptions)),
    })
    {
    }
}