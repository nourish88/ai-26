
using Juga.Observability.Extensions;


namespace Juga.Api.Extensions;
//this class is used to register Modular Monolith services with DDD and CQRS
public static class ModularMonolithApiServiceCollectionExtensions
{
    public static IServiceCollection AddModularJugaApi(this IServiceCollection services,
    WebApplicationBuilder builder, ModularApiOptions options, Action<AuthorizationOptions>? authorizationAction = null)
    {
        ServiceCollectionConfigurationsExtensions.CheckMediatr(builder.Configuration, options);

        IdentityModelEventSource.ShowPII = true;
        services.ConfigureLogging(builder.Configuration);
        //services.ConfigureUnitOfWork(configuration);
        services.AddAutoMapper(options.RegistrationAssemblies);
        //services.TryAddTracing(builder.Configuration);
        services.AddObservability(builder.Configuration);
        services.ConfigureSignalR(builder.Configuration);
        services.ConfigureAuth(builder.Configuration);
        services.RegisterFluentValidation(options.RegistrationAssemblies);
        services.RegisterMediatr(options.RegistrationAssemblies);
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.TryAddRateLimitingServices(builder.Configuration);
        services.RegisterServicesAutomatically(options, builder.Configuration);
        services.AddRepositories(options);
        services.AddApiClients(options);
        services.AddHttpContextAccessor();
        services.AddHttpClient().AddEndpointsApiExplorer();
        services.AddAuthorization(builder.Configuration, options, authorizationAction);
        services.AddEndpoints(builder.Configuration, options);
        services.AddSwaggerGen(c => { c.EnableAnnotations(); c.SchemaFilter<DateWithoutTimezoneSchemaFilter>(); });
        services.ConfigureApiVersioning();
        services.ConfigureVault(builder.Configuration);
        services.AddScoped<IUserContextProvider, ClientInfoProvider>();
        services.AddInternalUnitOfWorkServices();
        services.RegisterDomainEventDispatcher();

        services.ConfigureBackendServicePolicies(builder.Configuration, builder.Environment);
        services.ConfigureTaskScheduler(builder.Configuration);
        if (options.HealthCheck == true) services.AddHealthChecks();
        if (options.RegisterQueueServices)
        {
            services.TryAddQueueServices(builder.Configuration, options.RegistrationAssemblies);
        }

        return services;
    }
}

