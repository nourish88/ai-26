
using HealthChecks.UI.Client;

using Juga.Api.Middlewares;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Microsoft.AspNetCore.Routing;


namespace Juga.Api.Extensions;

/// <summary>
///     Provides extension methods for configuring the HTTP pipeline and services at the framework level.
/// </summary>
public static class ApiApplicationBuilderExtensions
{
    public static WebApplication UseJugaApi(this WebApplication app,
        IConfiguration configuration, IApiOptions options,
        bool healthCheck = false)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        if (IsRateLimitinEnabled(configuration))
        {
            app.UseRateLimiter();
        }

        SetLogging(app, configuration);
        app.UseExceptionHandler();
        app.UseMiddleware<UserContextProviderMiddleware>();

        if (ShouldUseAuthentication(configuration))
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
        //Endpointler controller ya da minimal api Swaggerdan önce register olmalı.
        ConfigureEndpoints(app, options, healthCheck);

        if (options.IsMinimal)
        {
            app.MapCarter();
        }
        ConfigureSwagger(app, configuration, options);

       

        return app;
    }

    #region Configurations

    private static void ConfigureSwagger(WebApplication app, IConfiguration configuration,
       IApiOptions options)
    {
        const string allowAnonymous = "Juga:Api:AllowAnonymous";


        app.UseSwagger(o =>
            {
                o.RouteTemplate = "swagger/{documentName}/" + (options.ApiName ?? "swagger") + ".json";
            })
      .UseSwaggerUI(setup =>
      {

          var descriptions = app.DescribeApiVersions();
          foreach (var description in descriptions)
          {
              setup.SwaggerEndpoint($"{description.GroupName}/{options.ApiName ?? "swagger"}.json",
                  description.GroupName.ToUpperInvariant());
          }


          setup.DocumentTitle = options.ApiName;

          if (!IsAllowAnonymous(configuration, allowAnonymous))
          {
              setup.OAuthClientId(configuration["Juga:Security:Jwt:ClientId"]);
              setup.OAuthClientSecret(configuration["Juga:Security:Jwt:ClientSecret"]);
              setup.OAuthAppName("Juga API");
              setup.OAuthScopeSeparator(" ");
              setup.OAuthUsePkce();
          }
      });
    }

    private static bool IsAllowAnonymous(IConfiguration configuration, string allowAnonymousKey)
    {
        return configuration[allowAnonymousKey] != null && configuration.GetValue<bool>(allowAnonymousKey);
    }

    private static bool IsRateLimitinEnabled(IConfiguration configuration)
    {
        return configuration.GetValue<bool?>("Juga:RateLimiting:Enable") == true;
    }

    private static bool ShouldUseAuthentication(IConfiguration configuration)
    {
        return !IsAllowAnonymous(configuration, "Juga:Api:AllowAnonymous");
    }

    private static void ConfigureEndpoints(WebApplication app, IApiOptions options, bool healthCheck)
    {

        app.UseEndpoints(endpoints =>
        {
            if (!options.IsMinimal)
                endpoints.MapControllers();

            MapHubs(endpoints, options);

            if (healthCheck)
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
        });

    }

    private static void MapHubs(IEndpointRouteBuilder endpoints, IApiOptions options)
    {
        if (options.HubList != null)
        {
            foreach (var item in options.HubList)
            {
                var method = typeof(HubEndpointRouteBuilderExtensions).GetMethod("MapHub",
                    [typeof(IEndpointRouteBuilder), typeof(string)]);
                var generic = method?.MakeGenericMethod(item);
                generic?.Invoke(null, [endpoints, $"/{item.Name}"]);
            }
        }
    }

    private static void SetLogging(IApplicationBuilder app, IConfiguration configuration)
    {
        var loggingOptions = new LoggingOptions();
        configuration.Bind(LoggingOptions.OptionsSection, loggingOptions);
        var serviceProvider = app.ApplicationServices;
        var isMediatrRegistered = serviceProvider.GetService<IMediator>() != null;
        //bool isMediatrEnabled = CommonHelpers.IsMediatrEnabled(configuration, options);

        if (!isMediatrRegistered)
        {
            app.UseRequestResponseLogging();
        }
    }

    private static bool IsTraditionalRequestResponseLoggingEnabled(LoggingOptions loggingOptions, bool isMediatrEnabled)
    {
        return loggingOptions.EnableRequestResponseLogging && isMediatrEnabled != true &&
               loggingOptions.LoggerType == LoggerType.Serilog;
    }

    #endregion Configurations
}