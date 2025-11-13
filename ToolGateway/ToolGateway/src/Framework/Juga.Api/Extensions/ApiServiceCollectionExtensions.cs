using Juga.Observability.Extensions;

namespace Juga.Api.Extensions;

public static class ApiServiceCollectionExtensions
{
  
    public static IServiceCollection AddJugaApi<TContext>(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment env, ApiOptions options
        , Action<DbContextOptionsBuilder>? dbContextoptionsAction = null, bool healthCheck = false, Action<AuthorizationOptions>? authorizationAction = null)
        where TContext : UnitOfWork
    {

        ServiceCollectionConfigurationsExtensions.CheckMediatr(configuration, options);
        var isMediatrEnabled = CommonHelpers.IsMediatrEnabled(configuration, options);

        IdentityModelEventSource.ShowPII = true;
        services.ConfigureLogging(configuration);
        services.ConfigureUnitOfWork(configuration);
        services.AddAutoMapper(options.RegistrationAssemblies);
        //services.TryAddTracing(configuration);
        services.AddObservability(configuration);
        services.ConfigureSignalR(configuration);
        services.ConfigureAuth(configuration);
        services.RegisterFluentValidation(options.RegistrationAssemblies);
        services.RegisterServicesAutomatically(options, configuration);
        if (isMediatrEnabled)
        {
            services.RegisterMediatr(options.RegistrationAssemblies);
            
        }
        else
        {
            services.AddInterceptedServices();
        }

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.TryAddRateLimitingServices(configuration);

        services.AddRepositories(options);
        services.AddApiClients(options);
        services.AddHttpContextAccessor();
        services.AddHttpClient().AddEndpointsApiExplorer();
        services.AddAuthorization(configuration, options, authorizationAction);
        services.AddEndpoints(configuration, options);
        services.AddSwaggerGen(c => { c.EnableAnnotations(); c.SchemaFilter<DateWithoutTimezoneSchemaFilter>(); });
        services.ConfigureApiVersioning();
        services.ConfigureVault(configuration);
        services.AddScoped<IUserContextProvider, ClientInfoProvider>();
        services.AddInternalUnitOfWorkServices();
        if (isMediatrEnabled)// sıralaması önemli olduğu için buarada
        {
            services.RegisterDomainEventDispatcher();
        }

        if (dbContextoptionsAction != null)
        {
            services.AddUnitOfWork<TContext>(dbContextoptionsAction);
        }
        else
        {
            services.AddUnitOfWork<TContext>(configuration);
        }

        services.ConfigureAuditLogs(configuration, options);
        services.ConfigureBackendServicePolicies(configuration, env);
        services.ConfigureTaskScheduler(configuration);
        if (healthCheck) services.AddHealthChecks();
        services.TryAddQueueServices<TContext>(configuration, options.RegistrationAssemblies);
        services.AddDbContextFactory<TContext>();
        return services;
    }
}