using System.Diagnostics;

namespace Juga.Tracing.Constants;

public class OpenTelemetryConstants
{
    public string ServiceName { get; set; } = null!;
    public string ServiceVersion { get; set; } = null!;
    public string ActivitySourceName { get; set; } = null!;
    public string ExporterUri { get; set; } = null!;
}

public static class ActivitySourceProvider
{
    public static ActivitySource Source = null!;
}