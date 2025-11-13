
using Juga.Api.Enums;

using Juga.Caching.InMemory.Configuration;


namespace Juga.Api.Helpers;

public static class ModulithProgramHelper
{
    public static void RegisterServices<TContext>(WebApiStartUpConfig config, ApiOptions options) where TContext : UnitOfWork
    {
        var projectName = GetProjectName();
        SerilogProgramRunnerExtensions.RunHost(config.Builder);
       
         RegisterJugaServices<TContext>(config, projectName,options);

        RegisterCaching(config);
      
    }
    public static void RegisterJugaServices<TContext>(WebApiStartUpConfig config,
        string projectName, ApiOptions options) where TContext : UnitOfWork
    {
       
        var builder = config.Builder;
        var conStr = builder.Configuration.GetConnectionString(config.ConnectionStringName);
        switch (config.BaseDbProvider)
        {
            case EfCoreDbProviders.PostgreSql:
                builder.Services.AddModule<TContext>(builder.Configuration, options, dbcontextOptions => dbcontextOptions.UseNpgsql(conStr, b => b.MigrationsAssembly(config.MigrationAssemblyName)));
                break;
            case EfCoreDbProviders.MsSql:
                builder.Services.AddModule<TContext>(builder.Configuration, options
                    , dbcontextOptions => dbcontextOptions.UseSqlServer(conStr, b => b.MigrationsAssembly(config.MigrationAssemblyName)));
                break;
            case EfCoreDbProviders.Monggo:
                break;
            case null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
        options.HubList = new List<Type>();
        options.ApiName = builder.Configuration[config.OpenApiNameSection];
        webApplication.UseJugaApi(builder.Configuration, options);
        Log.Information($"Using Environment: {webApplication.Environment.EnvironmentName}");
        return webApplication;
    }
}

