namespace Juga.Abstractions.Data.Entities;

/// <summary>
/// Lookup tabloları için kullanılır
/// </summary>
public interface ILookUpEntity : IHasId
{
    public string Description { get; set; }
}