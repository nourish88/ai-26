namespace Juga.Data.Dynamic;

public class SpecifiedFilter
{
    public string Field { get; set; }
    public string Operator { get; set; } = "eq";
    public string Logic { get; set; } = "and";
}