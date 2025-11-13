namespace Juga.Abstractions.Data.Entities;

/// <summary>
/// Long Id ye sahip olan entityler için kullanılır.
/// </summary>
public interface IHasId<T> : IEntity
{
    T Id { get; set; }
}

public interface IHasId : IEntity
{
    long Id { get; set; }
}