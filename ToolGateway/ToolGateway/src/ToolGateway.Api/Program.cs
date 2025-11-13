using Juga.Api.Extensions;
using Juga.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Serilog;
using System.Configuration;
using System.Reflection;
using ToolGateway.Api;
using ToolGateway.Api.Policies;
using ToolGateway.Application.Dtos;
using ToolGateway.Domain.Entities;
using ToolGateway.Infrastructure.Data.Repositories;

try
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    var domainAssembly = typeof(Todo).Assembly;
    var applicationAssembly = typeof(TodoDto).Assembly;
    var infrastructureAssembly = typeof(TodoRepository).Assembly;
    var apiAssembly = Assembly.GetExecutingAssembly();
    var options = new ModularApiOptions
    {
        HealthCheck = false,
        IsMinimal = true,
        Mediator = true,
        RegistrationAssemblies = [domainAssembly, applicationAssembly, infrastructureAssembly, applicationAssembly],
        AsyncRegistrationAssemblies = []
    };
    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        serverOptions.Limits.MaxRequestBodySize =
            builder.Configuration.GetValue("MaxFileUploadSizeInMB", 200) * 1024 * 1024; // Default 200 MB
    });
    builder.Services.AddModularJugaApi(builder, options,
        authorizationAction: (authorizationOpt) =>
        {            
            authorizationOpt.AddPolicy(PolicyNames.ApplicationPolicyName, policy => { policy.Requirements.Add(new AppRoleRequirement()); });//AI orchestrator application role           

        });

    //Module Specific Registrations like UnitOfWork, Logging, SAveChangesInterceptors, Audit Logging etc. 
    builder.Services.AddModule(builder);
    //Custom servisces

    // Custom repositories
    // Business

    //MCP Server
    builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly(applicationAssembly);

    builder.Services.AddScoped<IAuthorizationHandler, AppRoleHandler>();

    var app = builder.Build();

    options.IsMinimal = true;

    options.ApiName = builder.Configuration["Juga:OpenApi:Name"];

    //Common Middlewares with Juga
    app.UseJugaApi(builder.Configuration, options);

    //Module Specific Middlewares and Extensions.  
    app.UseModule();

    //MCP Path
    var conventionBuilder = app.MapMcp("/api/mcp");
    if (conventionBuilder != null && (builder.Configuration["Juga:Api:AllowAnonymous"] == null || !builder.Configuration.GetValue<bool>("Juga:Api:AllowAnonymous")))
    {
        conventionBuilder.RequireAuthorization(PolicyNames.ApplicationPolicyName);
    }


    await app.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    await Log.CloseAndFlushAsync().ConfigureAwait(false);
}
