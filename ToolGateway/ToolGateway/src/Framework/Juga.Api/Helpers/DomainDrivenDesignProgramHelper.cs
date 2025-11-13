using Asp.Versioning.ApiExplorer;

namespace Juga.Api.Helpers;

public static class DomainDrivenDesignProgramHelper
{
    public static void RegisterJugaServices<TContext>(WebApplicationBuilder builder1, IEnumerable<Assembly> enumerable,
        string projectName,
        string connectionStringName, string dbContextProjectName) where TContext : UnitOfWork
    {
        builder1.Services.AddJugaApi<TContext>(builder1.Configuration, builder1.Environment,
            new ApiOptions
            { RegistrationAssemblies = enumerable, AuditLogStoreType = AuditLogStoreType.SqlServer }
            , dbcontextOptions => dbcontextOptions.UseSqlServer(builder1.Configuration.GetConnectionString(connectionStringName), b => b.MigrationsAssembly($"{projectName}.{dbContextProjectName}")));
    }

    public static void RegisterServices<TContext>(WebApiStartUpConfig config) where TContext : UnitOfWork
    {
        SerilogProgramRunnerExtensions.RunHost(config.Builder);
        var projectName = GetProjectName();
        var assemblies = GetAssemblies(projectName);
        RegisterJugaServices<TContext>(config.Builder!, assemblies, projectName,
            config.ConnectionStringName, config.DbContextProjectName);
        RegisterCaching(config.Builder!, config.Redis,
            config.CachingMechanismSection, config.RedisEndpointNameSection);
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

    public static void RegisterCaching(WebApplicationBuilder builder, string redis, string mechanismPath,
        string redisEndpointSection)
    {
        var cachingMechanism = GetCachingMechanism(builder, mechanismPath);
        if (string.IsNullOrWhiteSpace(cachingMechanism)) return;
        if (cachingMechanism != redis) RegisterInMemoryCache(builder);
        else RegisterRedisCache(builder, redisEndpointSection);
    }

    public static void UseJugaMiddlewares(WebApplication webApplication, WebApplicationBuilder builder2,
        string openApiNameSection)
    {
        if (webApplication.Environment.IsDevelopment()) webApplication.UseDeveloperExceptionPage();
        var provider = webApplication.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        var apiOptions = new ApiOptions
        { HubList = new List<Type>(), ApiName = builder2.Configuration[openApiNameSection], Mediator = true };
        webApplication.UseJugaApi( builder2.Configuration, apiOptions);
        Log.Information($"Using Environment: {webApplication.Environment.EnvironmentName}");
    }
}