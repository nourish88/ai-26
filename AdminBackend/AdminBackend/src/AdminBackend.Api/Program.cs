using AdminBackend.Api;
using AdminBackend.Api.Middlewares;
using AdminBackend.Api.Policies;
using AdminBackend.Application.Business;
using AdminBackend.Application.Dtos;
using AdminBackend.Application.Repositories;
using AdminBackend.Application.Services.AI;
using AdminBackend.Application.Services.App;
using AdminBackend.Application.Services.Integrations;
using AdminBackend.Application.Services.Search;
using AdminBackend.Application.Settings;
using AdminBackend.Domain.Entities;
using AdminBackend.Infrastructure.Data.Repositories;
using AdminBackend.Infrastructure.Services.AI;
using AdminBackend.Infrastructure.Services.App;
using AdminBackend.Infrastructure.Services.DataManager;
using AdminBackend.Infrastructure.Services.FileStorage;
using AdminBackend.Infrastructure.Services.Integrations;
using AdminBackend.Infrastructure.Services.Oidc;
using AdminBackend.Infrastructure.Services.Search;
using AdminBackend.Infrastructure.Services.Search.IndexDefinitions;
using AdminBackend.Infrastructure.Services.Search.SearchEngines;
using AdminBackend.Infrastructure.Services.Workers;
using Juga.Api.Extensions;
using Juga.Api.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Serilog;
using System.Reflection;

try
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    var domainAssembly = typeof(LlmProvider).Assembly;
    var applicationAssembly = typeof(LlmProviderDto).Assembly;
    var infrastructureAssembly = typeof(LlmProviderRepository).Assembly;
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
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(AppSettings.SectionName));
    builder.Services.Configure<IndexSettings>(builder.Configuration.GetSection(IndexSettings.SectionName));
    builder.Services.Configure<EmbeddingSettings>(builder.Configuration.GetSection(EmbeddingSettings.SectionName));
    builder.Services.AddScoped<IAppService, AppService>();
    builder.Services.AddModularJugaApi(builder, options,
        authorizationAction: (authorizationOpt) =>
        {//policy.RequireClaim("email_verified", "true");
            authorizationOpt.AddPolicy(PolicyNames.AdminPolicyName, policy =>
            {
                var appSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<AppSettings>>();
                policy.RequireRole(appSettings.Value.AdminRoleName);//Admin ui role
            });
            authorizationOpt.AddPolicy(PolicyNames.ApplicationPolicyName, policy => { policy.Requirements.Add(new AppRoleRequirement()); });//AI orchestrator role
            authorizationOpt.AddPolicy(PolicyNames.EitherPolicy, policy => { policy.Requirements.Add(new EitherPolicyRequirement()); });//AI orchestrator or admin role

            //authorizationOpt.DefaultPolicy = authorizationOpt.GetPolicy(PolicyNames.AdminPolicyName);
        });
    builder.Services.AddMemoryCache();
    builder.Services.AddStillProcessingJobWorker();
    builder.Services.AddIndexingJobWorker();

    //Module Specific Registrations like UnitOfWork, Logging, SAveChangesInterceptors, Audit Logging etc. 
    builder.Services.AddModule(builder);
    //Custom servisces
    builder.Services.AddHttpClient();
    builder.Services.AddTransient<ILiteLlmIntegration, LiteLlmIntegration>();
    builder.Services.AddTransient<IEmbeddingService, LiteLlmEmbeddingService>();
    builder.Services.AddTransient<IEmbeddingServiceFactory, EmbeddingServiceFactory>();
    builder.Services.AddTransient<IIndexDefinition, ElasticSearchIndexDefinition>();
    builder.Services.AddTransient<IIndexDefinitionFactory, IndexDefinitionFactory>();
    builder.Services.AddTransient<ISearchEngine, ElasticSearchEngine>();
    builder.Services.AddTransient<ISearchEngineFactory, SearchEngineFactory>();
    builder.Services.AddStorage(builder.Configuration);
    builder.Services.AddDataManagerService(builder.Configuration);
    builder.Services.AddOidcService(builder.Configuration);


    // Custom repositories
    builder.Services.AddScoped<IApplicationFileStoreRepository, ApplicationFileStoreRepository>();
    // Business
    builder.Services.AddScoped<IApplicationBusiness, ApplicationBusiness>();
    builder.Services.AddScoped<IAuthorizationHandler, AppRoleHandler>();
    builder.Services.AddScoped<IAuthorizationHandler, EitherPolicyHandler>();

    var app = builder.Build();

    options.IsMinimal = true;

    options.ApiName = builder.Configuration["Juga:OpenApi:Name"];

    //Common Middlewares with Juga
    app.UseJugaApi(builder.Configuration, options);


    //Module Specific Middlewares and Extensions.  
    app.UseModule();


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
