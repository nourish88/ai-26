using Microsoft.EntityFrameworkCore;

namespace Juga.Data.Paging;

public static class IQueryablePaginateExtensions
{
    public static async Task<Paginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int index, int size, CancellationToken cancellationToken = default)
    {
        int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);

        var items = await source.Skip(index * size).Take(size).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);


        var list = new Paginate<T>
        {
            Items = items,
            Count = count,
            Index = index,
            Pages = (int)Math.Ceiling((count / (double)size)),
            Size = size
        };
        return list;
    }
    public static Paginate<T> ToPaginate<T>(this IQueryable<T> source, int index, int size)
    {
        int count = source.Count();

        var items = source.Skip(index * size).Take(size).ToList();


        var list = new Paginate<T>
        {
            Items = items,
            Count = count,
            Index = index,
            Pages = (int)Math.Ceiling((count / (double)size)),
            Size = size
        };
        return list;
    }
}