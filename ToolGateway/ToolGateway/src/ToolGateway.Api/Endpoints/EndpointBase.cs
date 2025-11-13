using Carter;

namespace ToolGateway.Api.Endpoints
{
    public abstract class EndpointBase : CarterModule
    {
        private bool authorizationEnabled;
        protected EndpointBase(IConfiguration configuration) :base()
        {
            SetAuthorization(configuration);
        }

        protected EndpointBase(IConfiguration configuration,string basePath) : base(basePath)
        {
            SetAuthorization(configuration);
        }

        private void SetAuthorization(IConfiguration configuration)
        {
            if (configuration["Juga:Api:AllowAnonymous"] == null ||
            !configuration.GetValue<bool>("Juga:Api:AllowAnonymous"))
            {
                authorizationEnabled = true;
            }
                if (authorizationEnabled)
            {
                RequireAuthorization();
            }
        }
    }
}
