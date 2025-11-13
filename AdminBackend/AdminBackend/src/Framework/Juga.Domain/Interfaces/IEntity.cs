
using Juga.Abstractions.Data.AuditProperties;

namespace Juga.Domain.Interfaces;

public interface IEntity : IHasCreateDate, IHasUpdateDate, IHasCreatedBy, IHasUpdatedBy, IHasCreatedAt, IHasUpdatedAt
{
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public string CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
}

public interface IEntity<T> : IEntity
{
    public T Id { get; set; }
}