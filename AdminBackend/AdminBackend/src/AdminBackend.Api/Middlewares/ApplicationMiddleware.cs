using AdminBackend.Application.Services.App;
using AdminBackend.Infrastructure.Logging;
using Juga.Abstractions.Client;
using Serilog.Context;

namespace AdminBackend.Api.Middlewares
{
    public class ApplicationMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IUserContextProvider userContextProvider, IAppService appService, ILogger<ApplicationMiddleware> logger)
        {
            string? authHeader = context.Request.Headers["app-identifier"];
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                var isUserHasAppRole = context.User.IsInRole(authHeader);
                var isEligable = true;
                var errorMessage = "Unauthorized";
                if (!isUserHasAppRole)
                {
                    isEligable = false;
                    logger.LogError($"{userContextProvider.ClientId} not have {authHeader} role.");
                }
                else
                {
                    appService.RequesterApplicationIdentifier = authHeader;
                }

                if (!isEligable)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync(errorMessage);
                    return;
                }
            }
            using (LogContext.Push(new AppContextEnricher(appService)))
            {
                await next(context);
            }
        }


    }
}
