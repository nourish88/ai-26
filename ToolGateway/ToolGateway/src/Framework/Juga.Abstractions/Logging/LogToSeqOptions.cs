namespace Juga.Abstractions.Logging;

public class LogToSeqOptions
{
    /// <summary>
    /// Seq sunucusunun adresi.
    /// </summary>
    public string Uri { get; set; }

    /// <summary>
    /// Log kayıtlarının oluşturulacağı minimum log seviyesi.Default Information.
    /// </summary>
    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
}