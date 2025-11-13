
using Juga.Api.Enums;
using Juga.Caching.InMemory.Configuration;

namespace Juga.Api.Helpers;

public static class CleanArcProgramHelper
{
    public static ApiOptions RegisterServices<TContext>(WebApiStartUpConfig config) where TContext : UnitOfWork
    {
        var projectName = GetProjectName();
        SerilogProgramRunnerExtensions.RunHost(config.Builder);
        var assemblies = GetAssemblies(projectName);
        var options = RegisterJugaServices<TContext>(config, assemblies, projectName);
        //TODO: Framework içine al. 
        RegisterCaching(config);
        return options;
    }
    public static void AddUnitOfWork<TContext>(WebApplicationBuilder webApplicationBuilder, string connectionStringName) where TContext : UnitOfWork
    {


        webApplicationBuilder.Services.AddAdditionalUnitOfWork<TContext>(dbcontextOptions =>
        {
            dbcontextOptions.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString(connectionStringName),
                b =>
                {
                    b.MigrationsAssembly($"{b}.Infrastructure");
                    b.CommandTimeout(300);
                });
        });
    }
    public static ApiOptions RegisterJugaServices<TContext>(WebApiStartUpConfig config,
        IEnumerable<Assembly> enumerable,
        string projectName) where TContext : UnitOfWork
    {
        var options = new ApiOptions
        {
            RegistrationAssemblies = enumerable,
            AuditLogStoreType = config.AuditLogStoreType,
            Mediator = true,
            IsMinimal = config.IsMinimal
        };
        var builder = config.Builder;
        var conStr = builder!.Configuration.GetConnectionString(config.ConnectionStringName);
        switch (config.BaseDbProvider)
        {
            case EfCoreDbProviders.PostgreSql:
                builder.Services.AddJugaApi<TContext>(builder.Configuration, builder.Environment,
                    options
                    , dbcontextOptions => dbcontextOptions.UseNpgsql(conStr, b => b.MigrationsAssembly($"{projectName}.{config.DbContextProjectName}")));
                break;
            case EfCoreDbProviders.MsSql:
                builder.Services.AddJugaApi<TContext>(builder.Configuration, builder.Environment,
                    options
                    , dbcontextOptions => dbcontextOptions.UseSqlServer(conStr, b => b.MigrationsAssembly($"{projectName}.{config.DbContextProjectName}")));
                break;
            case EfCoreDbProviders.Monggo:
                break;
            case null:
                break;
            default:
                throw  new ArgumentOutOfRangeException();
        }
        if (config?.AdditionalUnitOfWorkConfigs != null)
            RegisterAdditionalUnitOfWorks(config);
        return options;
    }
    public static void RegisterAdditionalUnitOfWorks(WebApiStartUpConfig config)
    {
        var builder = config.Builder ?? throw new InvalidOperationException("Builder is not initialized.");

        foreach (var uowConfig in config.AdditionalUnitOfWorkConfigs!)
        {
            var conStr = builder.Configuration.GetConnectionString(uowConfig.ConnectionStringName);

            builder.Services.AddAdditionalUnitOfWork<UnitOfWork>(dbcontextOptions =>
            {
                switch (uowConfig.DbProvider)
                {
                    case EfCoreDbProviders.PostgreSql:
                        dbcontextOptions.UseNpgsql(conStr, b =>
                        {
                            b.MigrationsAssembly($"{config.DbContextProjectName}.Infrastructure");
                            b.CommandTimeout(300);
                        });
                        break;
                    case EfCoreDbProviders.MsSql:
                        dbcontextOptions.UseSqlServer(conStr, b =>
                        {
                            b.MigrationsAssembly($"{config.DbContextProjectName}.Infrastructure");
                            b.CommandTimeout(300);
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
    public static string GetProjectName()
    {
        return Assembly.GetEntryAssembly()?.GetName().Name?.Split(".")[0] ?? "";
    }

    public static IList<Assembly> GetAssemblies(string projectName)
    {
        return Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll", SearchOption.TopDirectoryOnly)
            .Where(filePath => Path.GetFileName(filePath).StartsWith(projectName))
            .Select(Assembly.LoadFrom).ToList();
    }

    public static string? GetRedisEndPoint(WebApplicationBuilder builder, string path)
    {
        return builder.Configuration.GetSection(path).Value;
    }

    public static string GetCachingMechanism(WebApplicationBuilder builder, string cachingMechanismPath)
    {
        return builder.Configuration.GetSection(cachingMechanismPath).Value ?? "";
    }

    public static void RegisterInMemoryCache(WebApplicationBuilder builder)
    {
        builder.Services.AddDistributedMemoryCache(); // in memory implementasyonu
    }

    public static void RegisterRedisCache(WebApplicationBuilder builder, string redisEndpointSection)
    {
        var redisEndPoint = GetRedisEndPoint(builder, redisEndpointSection);
        builder.Services.AddStackExchangeRedisCache(opt => opt.Configuration = redisEndPoint);
    }

    public static void RegisterCaching(WebApiStartUpConfig config)
    {
        var cachingMechanism = GetCachingMechanism(config.Builder ?? throw new InvalidOperationException(), config.CachingMechanismSection);
        if (string.IsNullOrWhiteSpace(cachingMechanism)) return;
        if (cachingMechanism != config.Redis)
        {
            RegisterInMemoryCache(config.Builder);
            config.Builder.Services.AddInMemoryCache(config.Builder.Configuration);
        }
        else
        {
            RegisterRedisCache(config.Builder, config.RedisEndpointNameSection);
        }
    }

    public static WebApplication UseJugaMiddlewares(WebApplication webApplication, WebApiStartUpConfig config, ApiOptions options)
    {
        //if (config.BaseDbProvider == EfCoreDbProviders.PostgreSql)
        //{
        //    AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        //}


        if (webApplication.Environment.IsDevelopment()) webApplication.UseDeveloperExceptionPage();


        var builder = config.Builder!;
        options.IsMinimal = config.IsMinimal;
        options.HubList = [];
        options.ApiName = builder.Configuration[config.OpenApiNameSection];
        webApplication.UseJugaApi(builder.Configuration, options);
        Log.Information($"Using Environment: {webApplication.Environment.EnvironmentName}");
        return webApplication;
    }
}