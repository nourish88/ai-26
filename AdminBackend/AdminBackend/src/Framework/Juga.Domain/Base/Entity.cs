

using Juga.Domain.Interfaces;

namespace Juga.Domain.Base;

public abstract class Entity<T> : IEntity<T>, IEquatable<Entity<T>>
{
    public T Id { get; set; } 
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; }
    public string CreatedBy { get; set; } = "null user";
    public string? UpdatedBy { get; set; }
    public string CreatedAt { get; set; } = "null ip";
    public string? UpdatedAt { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj is not Entity<T> entity) return false;
        if (obj.GetType() != GetType()) return false;
        if (entity.Id == null) return false;
        return entity.Id.Equals(Id);
    }

    public bool Equals(Entity<T>? other)
    {
        if (other == null) return false;
        if (other is not Entity<T> entity) return false;
        if (other.GetType() != GetType()) return false;
        if (entity.Id == null) return false;
        return entity.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        if (Id == null) throw new ArgumentNullException(nameof(Id));
        return Id.GetHashCode();
    }
}