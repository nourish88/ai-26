namespace Juga.Abstractions.Caching.CacheManagement;

public interface ICacheKeySuffixSelector
{
    public string GetSuffix(object[] arguments);
}