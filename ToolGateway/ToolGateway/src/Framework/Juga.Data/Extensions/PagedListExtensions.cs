

using System.Linq.Expressions;
using System.Transactions;
namespace Juga.Data.Extensions;

/// <summary>
/// Sayfalama için yardımcı ek methodlar içerir.
/// </summary>
public static class PagedListExtensions
{
    /// <summary>
    /// Belirtilen sayfa numarası <paramref name="pageNumber"/> ve sayfa büyüklüğüne göre <paramref name="pageSize"/> kaynak listeyi <paramref name="source"/> <see cref="IPagedList{T}"/> ye dönüştürür.
    /// </summary>
    /// <typeparam name="T">Kaynak tipi.</typeparam>
    /// <param name="source">Kaynak.</param>
    /// <param name="pageNumber">Sayfa numarası.</param>
    /// <param name="pageSize">Sayfa büyüklüğü.</param>
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0)
        {
            throw new ArgumentException($"pageNumber: {pageNumber} <= 0 , pageNumber > 0 olmalı.");
        }

        var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await source.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

        var pagedList = new PagedList<T>()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = count,
            Items = items,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };

        return pagedList;
    }



    public static async Task<IPagedList<T>> ToPagedListWithNoLockAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber <= 0)
        {
            throw new ArgumentException($"pageNumber: {pageNumber} <= 0 , pageNumber > 0 olmalı.");
        }

        var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await source.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToListWithNoLockAsync(cancellationToken).ConfigureAwait(false);

        var pagedList = new PagedList<T>()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = count,
            Items = items,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };

        return pagedList;
    }


    public static async Task<List<T>> ToListWithNoLockAsync<T>(this IQueryable<T> query, CancellationToken cancellationToken = default, Expression<Func<T, bool>> expression = null)
    {
        List<T> result = default;
        using (var scope = CreateTrancation())
        {
            if (expression != null)
            {
                query = query.Where(expression);
            }
            result = await query.ToListAsync(cancellationToken);
            scope.Complete();
        }
        return result;
    }
    private static TransactionScope CreateTrancation()
    {
        return new TransactionScope(TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadUncommitted
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
    /// <summary>
    /// Belirtilen sayfa numarası <paramref name="pageIndex"/> ve sayfa büyüklüğüne göre <paramref name="pageSize"/> kaynak listeyi <paramref name="source"/> <see cref="IPagedList{T}"/> ye dönüştürür.
    /// </summary>
    /// <typeparam name="T">Kaynak tipi.</typeparam>
    /// <param name="source">Kaynak.</param>
    /// <param name="pageNumber">Sayfa numarası.</param>
    /// <param name="pageSize">Sayfa büyüklüğü.</param>
    public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageNumber, int pageSize) => new PagedList<T>(source, pageNumber, pageSize);
}