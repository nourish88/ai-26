namespace Juga.Abstractions.Caching;

public interface ICacheItem<T> : ICacheItem
{
    T Value { get; }
}

public interface ICacheItem
{
    string Key { get; }
    string Region { get; }
    string Policy { get; }
    TimeSpan Expire { get; }
    CacheExpirationTypeEnum ExpirationMode { get; }
}