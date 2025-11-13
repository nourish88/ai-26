using Juga.Mvc.ExceptionHandling;
using Juga.Mvc.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Juga.Mvc.Extensions;

public static class MvcExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseMvcExceptionHandler(this IApplicationBuilder builder)
    {
        var options = new MvcExceptionOptions();
        return builder.UseMiddleware<MvcExceptionMiddleware>(options);
    }

    public static IApplicationBuilder UseMvcExceptionHandler(this IApplicationBuilder builder,
        Action<MvcExceptionOptions> configureOptions)
    {
        var options = new MvcExceptionOptions();
        configureOptions(options);

        return builder.UseMiddleware<MvcExceptionMiddleware>(options);
    }
}