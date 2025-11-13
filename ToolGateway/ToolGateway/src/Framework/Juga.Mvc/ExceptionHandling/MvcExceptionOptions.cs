using Juga.Abstractions.ExceptionHandling;
using Microsoft.AspNetCore.Http;

namespace Juga.Mvc.ExceptionHandling;

public class MvcExceptionOptions
{
    public string ErrorViewName = "_Error";
    public Action<HttpContext, Exception, ErrorResult> AddResponseDetails { get; set; }
    public Func<Exception, Microsoft.Extensions.Logging.LogLevel> DetermineLogLevel { get; set; }
}