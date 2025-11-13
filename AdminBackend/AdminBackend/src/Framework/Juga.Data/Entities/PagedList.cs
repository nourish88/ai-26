using Juga.Abstractions.Data.Entities;
namespace Juga.Data.Entities;

/// <summary>
/// Sayfalama için kullanılır.
/// </summary>
public class PagedList<T> : IPagedList<T>
{
    /// <summary>.
    /// Sayfa numarası
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Bir sayfadaki max. kayıt sayısı
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Toplam kayıt sayısı.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Toplam sayfa sayısı.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Şuan ki sayfanın kayıtları.
    /// </summary>
    public IList<T> Items { get; set; }
    public dynamic? AdditionalData { get; set; }
    /// <summary>
    /// Önceki bir sayfa olup olmadığı bilgisi.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Sonraki bir sayfa olup olmadığı bilgisi.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// <see cref="PagedList{T}" /> sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="source">Kaynak.</param>
    /// <param name="pageNumber">Sayfa numarası.</param>
    /// <param name="pageSize">Sayfa büyüklüğü.</param>
    internal PagedList(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
        {
            throw new ArgumentException($"pageNumber: {pageNumber} <= 0 , pageNumber > 0 olmalı.");
        }

        if (source is IQueryable<T> querable)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = querable.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            Items = querable.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        }
        else
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            Items = source.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        }
    }

    /// <summary>
    /// <see cref="PagedList{T}" /> sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    internal PagedList() => Items = new T[0];
}