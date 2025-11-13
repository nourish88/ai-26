namespace Juga.Abstractions.Caching;

/// <summary>
/// Framework tarafında cache implementasyonlarında kullanılacak base sınıf
/// </summary>
public interface ICacheManager<T>
{
    #region[ADD]

    /// <summary>
    /// Belirtilen cache itemı cache e eklemek için kullanılır.
    /// </summary>
    /// <param name="cacheItem">Cache e eklenecek cache item.</param>
    /// <returns>Belirtilen cache item daha önce eklenmediyse true eklendiyse false</returns>
    public bool Add(ICacheItem<T> cacheItem);

    /// <summary>
    /// Key ile belirtilen value değerini cache e eklemek için kullanılır.
    /// </summary>
    /// <param name="key">Cache deki nesneyi belirlemek için kullanılan anahtar değeri</param>
    /// <param name="value">Cache de saklanacak değer.</param>
    /// <param name="policyName">Expiration policy.</param>
    /// <param name="region">Cache region.</param>
    /// <returns>Belirtilen cache item daha önce eklenmediyse true eklendiyse false</returns>
    public bool Add(string key, T value, string policyName = Constants.DefaultPolicy, string region = Constants.DefaultRegion);

    /// <summary>
    /// Key ile belirtilen value değerini cache e eklemek için kullanılır.
    /// </summary>
    /// <param name="key">Cache deki nesneyi belirlemek için kullanılan anahtar değeri.</param>
    /// <param name="value">Cache de saklanacak değer.</param>
    /// <param name="expire">Cache de saklanan nesnenin ne kadar zaman sonra yok olacağı.</param>
    /// <param name="expirationMode">Cache de saklanan nesnenin yok olma zamanının hesaplanma şekli<seealso cref="Abstractions.Caching.CacheExpirationTypeEnum"/></param>
    /// <param name="region">Cache region.</param>
    /// <returns>Belirtilen cache item daha önce eklenmediyse true eklendiyse false</returns>
    public bool Add(string key, T value, TimeSpan expire, CacheExpirationTypeEnum expirationMode, string region = Constants.DefaultRegion);

    #endregion[END_ADD]

    #region[ADD_OR_UPDATE]

    /// <summary>
    /// Belirtilen cache itemı cache daha önce eklenmediyse eklemek eklendi ise güncellemek için kullanılır.
    /// </summary>
    /// <param name="cacheItem">Cache e eklenecek cache item.</param>
    public void AddOrUpdate(ICacheItem<T> cacheItem);

    /// <summary>
    /// Key ile belirtilen value değerini daha önce eklenmediyse cache eklemek eklendi ise güncellemek için kullanılır.
    /// </summary>
    /// <param name="key">Cache deki nesneyi belirlemek için kullanılan anahtar değeri</param>
    /// <param name="value">Cache de saklanacak değer.</param>
    /// <param name="policyName">Expiration policy.</param>
    /// <param name="region">Cache region.</param>
    public void AddOrUpdate(string key, T value, string policyName = Constants.DefaultPolicy, string region = Constants.DefaultRegion);

    /// <summary>
    /// Key ile belirtilen value değerini daha önce eklenmediyse cache eklemek eklendi ise güncellemek için kullanılır.
    /// </summary>
    /// <param name="key">Cache deki nesneyi belirlemek için kullanılan anahtar değeri.</param>
    /// <param name="value">Cache de saklanacak değer.</param>
    /// <param name="expire">Cache de saklanan nesnenin ne kadar zaman sonra yok olacağı.</param>
    /// <param name="expirationMode">Cache de saklanan nesnenin yok olma zamanının hesaplanma şekli<seealso cref="Abstractions.Caching.CacheExpirationTypeEnum"/></param>
    /// <param name="region">Cache region.</param>
    public void AddOrUpdate(string key, T value, TimeSpan expire, CacheExpirationTypeEnum expirationMode, string region = Constants.DefaultRegion);

    #endregion[END_ADD_OR_UPDATE]

    #region[GET]

    /// <summary>
    /// Cachede saklanan nesneyi döndürmek için kullanılır.
    /// </summary>
    /// <param name="cacheItem">Cache eklenmiş cache item.</param>
    /// <returns>Cache eklenmiş nesne.</returns>
    public T Get(ICacheItem<T> cacheItem);

    /// <summary>
    /// Key ile belirtilen ve cachede saklanan nesneyi döndürmek için kullanılır.
    /// </summary>
    /// <param name="key">Cache deki nesneyi belirlemek için kullanılan anahtar değeri.</param>
    /// <param name="region">Cache region.</param>
    /// <returns>Cache eklenmiş nesne.</returns>
    public T Get(string key, string region = Constants.DefaultRegion);

    #endregion[END_GET]

    #region[EXISTS]

    /// <summary>
    /// Cachede saklanan bir nesne olup olmadığını belirlemek için kullanılır.
    /// </summary>
    /// <param name="cacheItem">Cache eklenmiş cache item.</param>
    /// <returns>Cachede saklanan nesne varsa true yoksa false</returns>
    public bool Exists(ICacheItem<T> cacheItem);

    /// <summary>
    /// Cachede key ile belirtilen bir nesne olup olmadığını belirtmek için kullanılır.
    /// </summary>
    /// <param name="key">Cache deki nesneyi belirlemek için kullanılan anahtar değeri.</param>
    /// <param name="region">Cache region.</param>
    /// <returns>Cachede saklanan nesne varsa true yoksa false</returns>
    public bool Exists(string key, string region = Constants.DefaultRegion);

    #endregion[END_EXISTS]

    #region[REMOVE]

    /// <summary>
    /// Cachede bulunan bir nesnenin cacheden silinmesi için kullanılır.
    /// </summary>
    /// <param name="cacheItem">Cache eklenmiş cache item.</param>
    /// <returns>Cache bulunur ve silinse true yoksa false döner.</returns>
    public bool Remove(ICacheItem<T> cacheItem);

    /// <summary>
    /// Cacheden key ile belirtilen nesnenin silinmesi için kullanılır.
    /// </summary>
    /// <param name="key">Cache deki nesneyi belirlemek için kullanılan anahtar değeri.</param>
    /// <param name="region">Cache region.</param>
    /// <returns>Cache bulunur ve silinse true yoksa false döner.</returns>
    public bool Remove(string key, string region = Constants.DefaultRegion);

    #endregion[END_REMOVE]

    #region[CLEAR]

    /// <summary>
    /// Tüm cache i temizlemek için kullanılır.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Cache de bulunan belirli bir regiondaki tüm nesneleri temizlemek için kullanılır.
    /// </summary>
    /// <param name="cacheItem">Cache eklenmiş cache item.</param>
    public void ClearRegion(ICacheItem<T> cacheItem);

    /// <summary>
    /// Belirtilmiş regiondaki tüm cache nesnelerini temizlemek için kullanılır.
    /// </summary>
    /// <param name="region">Cache region.</param>
    public void ClearRegion(string region = Constants.DefaultRegion);

    #endregion[END_CLEAR]
}