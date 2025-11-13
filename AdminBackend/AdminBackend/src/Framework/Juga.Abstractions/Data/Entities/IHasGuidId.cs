namespace Juga.Abstractions.Data.Entities;

public interface IHasGuidId : IEntity
{
    Guid Id { get; set; }
}