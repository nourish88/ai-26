namespace Juga.Data.Paging;

public class PageResponse<T> : BasePageableModel
{
    private IList<T> _items;

    public IList<T> Items
    {
        get => _items ??= [];
        set => _items = value;
    }
}