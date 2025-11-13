namespace Juga.Data.Paging;

public class Paginate<T>
{
    public int? Size { get; set; }
    public int? Index { get; set; }


    public int Count { get; set; }
    public int Pages { get; set; }
    public IList<T> Items { get; set; } = Array.Empty<T>();
    public bool HasNext => Index + 1 < Pages;
    public bool HasPrevious => Index > 0;
}