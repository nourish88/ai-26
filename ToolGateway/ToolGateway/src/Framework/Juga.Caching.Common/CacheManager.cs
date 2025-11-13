using Juga.Abstractions.Caching;
namespace Juga.Caching.Common;

public class CacheManager<T>(CacheManager.Core.ICacheManager<T> cacheManager,
        ICacheExpirationManager cacheExpirationManager)
    : ICacheManager<T>
{
    #region[ADD]

    public bool Add(ICacheItem<T> cacheItem)
    {
        var expirationSettings = cacheExpirationManager.GetCacheExpirationSetting(cacheItem);
        var add= Add(cacheItem.Key,
            cacheItem.Value,
            expirationSettings.ExpirationTime,
            expirationSettings.ExpirationMode,
            GetRegionOrDefault(cacheItem));
        return add;
    }
    public bool Add(string key, T value, string policyName = Constants.DefaultPolicy, string region = Constants.DefaultRegion)
    {
        var expirationSettings = cacheExpirationManager.GetCacheExpirationSetting(key, policyName);
        var add=  Add(key,
            value,
            expirationSettings.ExpirationTime,
            expirationSettings.ExpirationMode,
            GetRegionOrDefault(region));
        return add;
    }
    public bool Add(string key, T value, TimeSpan expire, CacheExpirationTypeEnum expirationMode, string region = Constants.DefaultRegion)
    {
        var expireMode = expirationMode.ToExpirationMode();
        return cacheManager.Add(new CacheManager.Core.CacheItem<T>(key, region, value, expireMode, expire));
    }

    #endregion[END_ADD]

    #region[ADD_OR_UPDATE]

    public void AddOrUpdate(ICacheItem<T> cacheItem)
    {
        var expirationSettings = cacheExpirationManager.GetCacheExpirationSetting(cacheItem);
        AddOrUpdate(cacheItem.Key,
            cacheItem.Value,
            expirationSettings.ExpirationTime,
            expirationSettings.ExpirationMode,
            GetRegionOrDefault(cacheItem));
    }
    public void AddOrUpdate(string key, T value, string policyName = Constants.DefaultPolicy, string region = Constants.DefaultRegion)
    {
        var expirationSettings = cacheExpirationManager.GetCacheExpirationSetting(key, policyName);
        AddOrUpdate(key,
            value,
            expirationSettings.ExpirationTime,
            expirationSettings.ExpirationMode,
            GetRegionOrDefault(region));
    }
    public void AddOrUpdate(string key, T value, TimeSpan expire, CacheExpirationTypeEnum expirationMode, string region = Constants.DefaultRegion)
    {
        var expireMode = expirationMode.ToExpirationMode();
        cacheManager.Put(new CacheManager.Core.CacheItem<T>(key, region, value, expireMode, expire));
    }

    #endregion[END_ADD_OR_UPDATE]

    #region[GET]

    public T Get(ICacheItem<T> cacheItem)
    {
        return Get(cacheItem.Key, cacheItem.Region);
    }

    public T Get(string key, string region = Constants.DefaultRegion)
    {
        var get=  cacheManager.Get<T>(key, GetRegionOrDefault(region));
        return get;
    }

    #endregion[END_GET]

    #region[EXISTS]

    public bool Exists(ICacheItem<T> cacheItem)
    {
        return Exists(cacheItem.Key, cacheItem.Region);
    }
    public bool Exists(string key, string region = Constants.DefaultRegion)
    {
        return cacheManager.Exists(key, GetRegionOrDefault(region));
    }

    #endregion[END_EXISTS]

    #region[REMOVE]

    public bool Remove(ICacheItem<T> cacheItem)
    {
        return Remove(cacheItem.Key, cacheItem.Region);
    }

    public bool Remove(string key, string region = Constants.DefaultRegion)
    {
        return cacheManager.Remove(key, GetRegionOrDefault(region));
    }

    #endregion[END_REMOVE]

    #region[CLEAR]

    public void Clear()
    {
        cacheManager.Clear();
    }

    public void ClearRegion(ICacheItem<T> cacheItem)
    {
        cacheManager.ClearRegion(cacheItem.Region);
    }
    public void ClearRegion(string region = Constants.DefaultRegion)
    {
        cacheManager.ClearRegion(GetRegionOrDefault(region));
    }

    #endregion[END_CLEAR]

    private static string GetRegionOrDefault(ICacheItem<T> cacheItem)
    {
        //TODO: conflict
        return GetRegionOrDefault(cacheItem.Region);
    }
    private static string GetRegionOrDefault(string region)
    {
        return string.IsNullOrEmpty(region) ? Constants.DefaultRegion : region;
    }
}