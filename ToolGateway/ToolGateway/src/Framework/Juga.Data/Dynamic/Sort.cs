namespace Juga.Data.Dynamic;

public class Sort(string field, string direction)
{
    public string Field { get; set; } = field;
    public string Direction { get; set; } = direction;

    public Sort() : this(string.Empty, "asc")
    {
    }
}