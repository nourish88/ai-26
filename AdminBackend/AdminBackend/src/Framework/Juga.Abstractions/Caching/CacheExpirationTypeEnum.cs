namespace Juga.Abstractions.Caching;

public enum CacheExpirationTypeEnum
{
    /// <summary>
    /// Tam olarak belirtilen zaman kadar sonra nesnenin cacheden silinmesini sağlar.
    /// </summary>
    Absolute,

    /// <summary>
    /// Belirtilen zaman kadar erişim sağlanmadığında nesnenin cacheden silinmesini sağlar.
    /// </summary>
    Sliding,

    /// <summary>
    /// Cache ayarlarında default verilmiş değerin kullanılmasını sağlar.
    /// </summary>
    Default,

    /// <summary>
    /// Nesnenin cacheden otomatik olarak silinmemesini sağlar.
    /// </summary>
    None
}