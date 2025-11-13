
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Juga.Api.Extensions;

public static class ServiceCollectionConfigurationsExtensions
{
    public static void CheckMediatr(IConfiguration configuration, IApiOptions options)
    {
        var conf = configuration.GetValue<bool?>("Juga:CQRS:Enabled");
        if (options.Mediator == null)
        {
            if (conf is null) // ikisi de null ise false
                options.Mediator = false;

            options.Mediator = conf;
            return;
        }

        if (conf != null && conf != options.Mediator)
            throw new AmbiguousActionException(
                "Mediatr Activation Confliction. Check Api Options and Appsetting CQRS sections to be consistent.");
    }

    public static void ConfigureVault(this IServiceCollection services, IConfiguration configuration)
    {
        IAuthMethodInfo authMethod = new TokenAuthMethodInfo(configuration["Juga:KeyVault:Token"]);
        var vaultClientSettings = new VaultClientSettings(configuration["Juga:KeyVault:Address"], authMethod);

        services.AddSingleton(vaultClientSettings);
        services.AddTransient<IVaultClient, VaultClient>();
        services.AddTransient<IVaultProvider, HashiVaultProvider>();
    }

    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenerationOptions>();
        if (configuration["Juga:Api:AllowAnonymous"] == null ||
            !configuration.GetValue<bool>("Juga:Api:AllowAnonymous"))
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    configuration.Bind("Juga:Security:Jwt", options);
                    options.Events = new JwtBearerEvents()
                    {

                        OnTokenValidated = ctx =>
                        {
                            List<AuthenticationToken> tokens = ctx.Properties!.GetTokens().ToList();
                            ClaimsIdentity claimsIdentity = (ClaimsIdentity)ctx.Principal!.Identity!;

                            var realm_access = claimsIdentity.FindFirst((claim) => claim.Type == "realm_access")?.Value;
                            var resource_access = claimsIdentity.FindFirst((claim) => claim.Type == "resource_access")?.Value;
                            if (!string.IsNullOrEmpty(realm_access))
                            {
                                JObject realmAccessObj = JObject.Parse(realm_access);
                                var realmRoleAccess = realmAccessObj.GetValue("roles");
                                foreach (JToken role in realmRoleAccess!)
                                {
                                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                                }
                            }


                            if (!string.IsNullOrEmpty(resource_access))
                            {
                                JObject resourceAccessObj = JObject.Parse(resource_access);
                                var resourceClient = resourceAccessObj.GetValue(configuration["Juga:Security:Jwt:ClientId"]);
                                if (resourceClient != null)
                                {
                                    JObject clientAccessObj = JObject.Parse(resourceClient.ToString());
                                    var clientRoleAccess = clientAccessObj.GetValue("roles");
                                    foreach (JToken role in clientRoleAccess!)
                                    {
                                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                                    }
                                }
                            }

                            return Task.CompletedTask;
                        }

                    };
                });
    }

   

    public static void ConfigureSignalR(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("Juga:SignalR:EnableScaling"))
            services.AddSignalR().AddStackExchangeRedis(o =>
            {
                o.ConnectionFactory = async writer =>
                {
                    var config = new ConfigurationOptions
                    {
                        AbortOnConnectFail = false
                    };
                    config.EndPoints.Add(
                        IPAddress.Parse(configuration.GetValue<string>("Juga:SignalR:StateStore:Address") ??
                                        string.Empty),
                        configuration.GetValue<int>("Juga:SignalR:StateStore:Port"));
                    config.SetDefaultPorts();
                    var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
                    connection.ConnectionFailed += (_, e) => { Console.WriteLine("Connection to Redis failed."); };

                    if (!connection.IsConnected) Console.WriteLine("Did not connect to Redis.");

                    return connection;
                };
            });
        else
            services.AddSignalR();
    }

    public static void ConfigureBackendServicePolicies(this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment env)
    {
        var backendSections = configuration.GetSection("Juga:Service:Backends");

        var backends = backendSections?.Get<BackendModel[]>() ?? null;
        if (backends != null)
        {
            //TODO: Backend servisleri böyle tanımlamıyoruz.
            foreach (var section in backends)
                if (!string.IsNullOrWhiteSpace(section.Name))
                {
                    var httpClientConfigration = services
                        .AddHttpClient(section.Name, c => { c.BaseAddress = new Uri(section.Address); })
                        .AddPolicyHandler(p => Policy
                            .Handle<Exception>(res => p.Method == HttpMethod.Get || p.Method == HttpMethod.Put)
                            .OrTransientHttpStatusCode()
                            .WaitAndRetryAsync(0, retryAttempt =>
                                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt
                                )))).AddPolicyHandler(p => HttpPolicyExtensions
                            .HandleTransientHttpError()
                            .CircuitBreakerAsync(1, TimeSpan.FromSeconds(30)));
                    if (env.IsDevelopment())
                        httpClientConfigration.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                        {
                            ClientCertificateOptions = ClientCertificateOption.Manual,
                            ServerCertificateCustomValidationCallback =
                                (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
                        });
                }

            services.AddTransient<ITokenProvider, HttpContextTokenProvider>();
            services.AddTransient<IHttpClientProvider, HttpClientProvider>();
        }
    }

    public static void ConfigureAuditLogs(this IServiceCollection services, IConfiguration configuration,
        ApiOptions options)
    {
        services.AddScoped<IAuditBehaviourService, AuditBehaviourService>();
        switch (options.AuditLogStoreType)
        {
            case AuditLogStoreType.SqlServer:
                services.ConfigureDataAuditSqlServer(configuration);
                break;

            case AuditLogStoreType.Elastic:
                services.ConfigureDataAuditElastic(configuration);
                break;
            case AuditLogStoreType.PostgreSql:
                {
                    services.ConfigureDataAuditPostgreSql(configuration);
                    break;
                }
            case AuditLogStoreType.None:
                break;

            default:
                services.ConfigureDataAuditSqlServer(configuration);
                break;
        }
    }
    public static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration
        , IApiOptions apiOptions, Action<AuthorizationOptions>? authorizationAction)
    {       

        if (authorizationAction != null)
        {
            services.AddAuthorization(authorizationAction);
        }

        if (!apiOptions.IsMinimal && authorizationAction == null)
        {
            if (configuration["Juga:Api:AllowAnonymous"] != null &&
                    configuration.GetValue<bool>("Juga:Api:AllowAnonymous")) return services;
            services.AddAuthorization(opt =>
            {
                opt.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            });

        }
        return services;
    }
    public static void AddEndpoints(this IServiceCollection services, IConfiguration configuration,
        IApiOptions options)
    {
        if (!options.IsMinimal)
            services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatString = "dd-MM-yyyy HH:mm:ss";
            });
        else
        {
            services.AddCarter(configurator: config =>
            {
                config.WithValidatorLifetime(ServiceLifetime.Scoped);
                foreach (var assembly in options.RegistrationAssemblies)
                {
                    var modules = assembly.GetTypes()
                        .Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();
                    config.WithModules(modules);
                }
            });

            //services.AddCarter();
        }

    }

    public static void ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(), new QueryStringApiVersionReader(), new MediaTypeApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }
}