namespace Juga.Abstractions.Data.AuditProperties;

public interface IHasSomeAudit : IHasCreateDate, IHasUpdateDate, IHasCreatedBy, IHasUpdatedBy, IHasCreatedAt, IHasUpdatedAt
{
}