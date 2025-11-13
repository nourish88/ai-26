using AdminBackend.Application.Services.App;
using AdminBackend.Application.Settings;
using AdminBackend.Infrastructure.Services.App;
using Juga.Abstractions.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AdminBackend.Api.Policies
{
    public class EitherPolicyRequirement : IAuthorizationRequirement
    {
       
    }
    public class EitherPolicyHandler : AuthorizationHandler<EitherPolicyRequirement>
    {
        private readonly ILogger<AppRoleHandler> logger;
        private readonly IOptions<AppSettings> settings;
        private readonly IUserContextProvider userContextProvider;
        private readonly IAppService appService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public EitherPolicyHandler(ILogger<AppRoleHandler> logger
            , IOptions<AppSettings> settings
            , IUserContextProvider userContextProvider
            , IAppService appService
            , IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger;
            this.settings = settings;
            this.userContextProvider = userContextProvider;
            this.appService = appService;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EitherPolicyRequirement requirement)
        {
            if(context.User.IsInRole(settings.Value.AdminRoleName))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else
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
    }
}
