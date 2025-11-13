using Juga.Abstractions.ExceptionHandling;

namespace Juga.Api.ExceptionHandling;

public class ApiExceptionOptions
{
    public Action<HttpContext, Exception, ErrorResult> AddResponseDetails { get; set; }
    public Func<Exception, Microsoft.Extensions.Logging.LogLevel> DetermineLogLevel { get; set; }
}