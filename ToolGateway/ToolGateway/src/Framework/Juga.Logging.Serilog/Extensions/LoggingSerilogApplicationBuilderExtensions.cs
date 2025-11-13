using Juga.Logging.Serilog.Middlewares;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Juga.Logging.Serilog.Extensions;

public static class LoggingSerilogApplicationBuilderExtensions
{

    // Mediatr Yoksa Aktif Olur
    public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
        app.UseMiddleware<RequestResponseLoggingMiddleware>();
        return app;
    }
}