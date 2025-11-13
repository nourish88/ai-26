using Juga.Abstractions.Client;
using Juga.Abstractions.ExceptionHandling;
using Juga.Logging.Serilog.Enrichers;
using Juga.Mvc.ExceptionHandling;
using Juga.Mvc.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;

namespace Juga.Mvc.Middlewares;

public class MvcExceptionMiddleware(MvcExceptionOptions options, RequestDelegate next,
    ILogger<MvcExceptionMiddleware> logger)
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

        var level = options.DetermineLogLevel?.Invoke(exception) ?? LogLevel.Error;
        logger.Log(level, exception, innerExMessage, error);
        if (context.Request.IsAjaxRequest())
        {
            var result = JsonConvert.SerializeObject(error);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = error.StatusCode;
            return context.Response.WriteAsync(result);
        }

        var viewResult = GetView(error);
        var executor = GetExecuter(context);
        var actionContext = GetActionContext(context);
        return executor.ExecuteAsync(actionContext, viewResult);
    }

    private string GetInnermostExceptionMessage(Exception exception)
    {
        if (exception.InnerException != null)
            return GetInnermostExceptionMessage(exception.InnerException);

        return exception.Message;
    }

    private ViewResult GetView(ErrorResult error)
    {
        var viewResult = new ViewResult
        {
            ViewName = options.ErrorViewName
        };

        var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(),
            new ModelStateDictionary())
        {
            Model = error
        };
        viewResult.ViewData = viewDataDictionary;
        return viewResult;
    }

    private IActionResultExecutor<ViewResult> GetExecuter(HttpContext context)
    {
        return context.RequestServices.GetRequiredService<IActionResultExecutor<ViewResult>>();
    }

    private ActionContext GetActionContext(HttpContext context)
    {
        var routeData = context.GetRouteData() ?? new RouteData();
        return new ActionContext(context, routeData, new ActionDescriptor());
    }
}