using Juga.Caching.Common.Configuration;

namespace Juga.Caching.InMemory.Configuration;

/// <summary>
/// InMemory cache konfigurasyonu için kullanılır.
/// </summary>
public class InMemoryCachingOptions : CachingOptions
{
    /// <summary>
    /// Configuration Section.
    /// </summary>
    public const string ConfigurationSection = "Juga:Caching:InMemory";
    /// <summary>
    /// Serializer olarak json serializer kullanılması için kullanılır.
    /// </summary>
    public virtual bool EnableJsonSerializer { get; set; }
}