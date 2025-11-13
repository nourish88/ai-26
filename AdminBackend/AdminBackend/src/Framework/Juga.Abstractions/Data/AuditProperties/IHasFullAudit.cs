namespace Juga.Abstractions.Data.AuditProperties
{
    public interface IHasFullAudit : IHasSomeAudit, IHasCreatedByUserCode, IHasUpdatedByUserCode
    {
    }
}