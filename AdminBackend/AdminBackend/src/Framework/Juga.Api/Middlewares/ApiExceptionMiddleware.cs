using Juga.Abstractions.ExceptionHandling;
using Juga.Api.ExceptionHandling;
using Juga.Logging.Serilog.Enrichers;
using Newtonsoft.Json;
using Serilog.Context;

namespace Juga.Api.Middlewares;

public class ApiExceptionMiddleware(ApiExceptionOptions options, RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var userContextProvider = context.RequestServices.GetService<IUserContextProvider>();
            using (LogContext.Push(new UserContextEnricher(userContextProvider)))
            {
                await HandleExceptionAsync(context, ex);
            }
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var error = new ErrorResult();

        options.AddResponseDetails?.Invoke(context, exception, error);

        var innerExMessage = GetInnermostExceptionMessage(exception);

        Log.Error(exception: exception, innerExMessage);

        //Log.ForContext("ErrorBy", userContextProvider?.ClientId??"").ForContext("ErrorIp", userContextProvider?.ClientIp ?? "").Error(exception:exception, innerExMessage);

        error.ErrorMessage = innerExMessage;
        error.StackTrace = exception.StackTrace;
        error.Source = exception.Source;

        try
        {
            var result = JsonConvert.SerializeObject(error);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = error.StatusCode;
            return context.Response.WriteAsync(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private string GetInnermostExceptionMessage(Exception exception)
    {
        while (true)
        {
            if (exception.InnerException != null)
            {
                exception = exception.InnerException;
                continue;
            }

            return exception.Message;
            break;
        }
    }
}