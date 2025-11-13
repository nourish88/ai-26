using Juga.Abstractions.Client;
using Microsoft.AspNetCore.Authorization;

namespace ToolGateway.Api.Policies
{
    public class AppRoleHandler : AuthorizationHandler<AppRoleRequirement>
    {
        private readonly ILogger<AppRoleHandler> logger;
        private readonly IUserContextProvider userContextProvider;
        private readonly IHttpContextAccessor httpContextAccessor;
        public AppRoleHandler(
            ILogger<AppRoleHandler> logger
            , IUserContextProvider userContextProvider
            , IHttpContextAccessor httpContextAccessor)

        {
            this.logger = logger;
            this.userContextProvider = userContextProvider;
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
                if (context.FailureReasons != null)
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
