
using Juga.Observability.Extensions;


namespace Juga.Api.Extensions
{/// <summary>
/// This class is for projects which use DDD and MEdiatr. Test amaçlı yapılmıştır. Şuan kullanılmıyor.
/// </summary>
    public  static  class ApiWithMediatrServiceCollectionExtensions
    {
        public static IServiceCollection AddJugaApiWithMediatr<TContext>(this IServiceCollection services,
        IConfiguration configuration, IWebHostEnvironment env, ApiOptions options
        , Action<DbContextOptionsBuilder>? dbContextoptionsAction = null, bool healthCheck = false, Action<AuthorizationOptions>? authorizationAction = null)
        where TContext : UnitOfWork
        {
            ServiceCollectionConfigurationsExtensions.CheckMediatr(configuration, options);
            IdentityModelEventSource.ShowPII = true;
            services.ConfigureLogging(configuration);
            services.ConfigureUnitOfWork(configuration);
            services.AddAutoMapper(options.RegistrationAssemblies);
            //services.TryAddTracing(configuration);
            services.AddObservability(configuration);
            services.ConfigureSignalR(configuration);
            services.ConfigureAuth(configuration); 
            services.RegisterFluentValidation(options.RegistrationAssemblies);
            services.RegisterMediatr(options.RegistrationAssemblies);
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            services.TryAddRateLimitingServices(configuration);
            services.RegisterServicesAutomatically(options, configuration);
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
            services.RegisterDomainEventDispatcher();
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
}
