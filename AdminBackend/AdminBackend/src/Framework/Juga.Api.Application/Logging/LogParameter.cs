namespace Juga.Api.Application.Logging;

public class LogParameter(string name, object value, string type)
{
    public LogParameter() : this(string.Empty, string.Empty, string.Empty)
    {
    }

    public string Name { get; set; } = name;
    public object Value { get; set; } = value;
    public string Type { get; set; } = type;
}