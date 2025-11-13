using AdminBackend.Application.Services.App;
using AdminBackend.Infrastructure.Logging;
using Juga.Abstractions.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace AdminBackend.Api.Policies
{
    public class AppRoleHandler : AuthorizationHandler<AppRoleRequirement>
    {
        private readonly ILogger<AppRoleHandler> logger;
        private readonly IUserContextProvider userContextProvider;
        private readonly IAppService appService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public AppRoleHandler(
            ILogger<AppRoleHandler> logger
            , IUserContextProvider userContextProvider
            , IAppService appService
            , IHttpContextAccessor httpContextAccessor) 
        
        {
            this.logger = logger;
            this.userContextProvider = userContextProvider;
            this.appService = appService;
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AppRoleRequirement requirement)
        {
            string? authHeader = httpContextAccessor.HttpContext?.Request.Headers["app-identifier"];
            var isEligable = false;
            var error = "";
            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                var isUserHasAppRole = context.User.IsInRole(authHeader);
                
                if (!isUserHasAppRole)
                {
                    error = $"{userContextProvider.ClientId} not have {authHeader} role.";
                    
                }
                else
                {
                    isEligable = true;
                    appService.RequesterApplicationIdentifier = authHeader;
                }                
            }
            else
            {
                error = $"{userContextProvider.ClientId} not have app-identifier header.";
                isEligable = false;
            }

            if (!isEligable)
            {
                logger.LogError(error);
                var failReason = new AuthorizationFailureReason(this, error);
                if ( context.FailureReasons!=null )
                {
                    context.FailureReasons.ToList().Add(failReason);
                }
                return Task.CompletedTask;
            }
            context.Succeed(requirement);   
            return Task.CompletedTask;
        }
    }

    public class AppRoleRequirement : IAuthorizationRequirement
    {

    }
}
