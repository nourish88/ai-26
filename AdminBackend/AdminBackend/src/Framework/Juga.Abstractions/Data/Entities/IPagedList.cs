namespace Juga.Abstractions.Data.Entities;

/// <summary>
/// Sayfalama için gerekli olan interface i sağlar.
/// </summary>
public interface IPagedList<T>
{
    /// <summary>.
    /// Sayfa numarası
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// Bir sayfadaki max. kayıt sayısı
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Toplam kayıt sayısı.
    /// </summary>
    int TotalCount { get; }

    /// <summary>
    /// Toplam sayfa sayısı.
    /// </summary>
    int TotalPages { get; }

    /// <summary>
    /// Şuan ki sayfanın kayıtları.
    /// </summary>
    IList<T> Items { get; }

    /// <summary>
    /// Önceki bir sayfa olup olmadığı bilgisi.
    /// </summary>
    bool HasPreviousPage { get; }

    /// <summary>
    /// Sonraki bir sayfa olup olmadığı bilgisi.
    /// </summary>
    bool HasNextPage { get; }
}