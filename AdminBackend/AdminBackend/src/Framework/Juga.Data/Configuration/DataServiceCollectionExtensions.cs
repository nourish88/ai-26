
using Juga.Data.Interceptors;
using System.Configuration;

namespace Juga.Data.Configuration;

public static class DataServiceCollectionExtensions
{
    /// <summary>
    ///     UnitOfWorkOptions konfigurasyonunun kaydı için kullanılır.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        return services.Configure<UnitOfWorkOptions>(
            configuration.GetSection(UnitOfWorkOptions.UnitOfWorkOptionsSection));
    }

    /// <summary>
    ///     UnitOfWorkOptions konfigurasyonunun kaydı için kullanılır.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureUnitOfWork(this IServiceCollection services,
        Action<UnitOfWorkOptions> action)
    {
        return services.Configure(action);
    }
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    /// 

    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : UnitOfWork
    {
        services.AddInternalUnitOfWorkServices();
        var unitOfWorkOptions = new UnitOfWorkOptions();
        configuration.Bind(UnitOfWorkOptions.UnitOfWorkOptionsSection, unitOfWorkOptions);
        Action<DbContextOptionsBuilder> optionsAction = GetDbContextOptions(unitOfWorkOptions);
        if (optionsAction != null)
        {
            services.AddDbContextOptions<TContext>((p, b) => optionsAction(b));
        }
        services.AddScoped<IUnitOfWork, TContext>();
        return services;
    }
    /// <summary>
    ///     UnitOfWork servisi ve bileşenlerini eklemek için kullanılır.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction = null) where TContext : UnitOfWork
    {
       
        if (optionsAction != null) services.AddDbContextOptions<TContext>((p, b) => optionsAction(b));
        services.AddDbContextFactory<TContext>(lifetime: ServiceLifetime.Scoped);
        string name = typeof(TContext).Name;
        services.AddScoped<IUnitOfWork, TContext>();
        services.AddKeyedScoped<IUnitOfWork, TContext>(name);

        return services;
    }

    /// <summary>
    ///     UnitOfWork servisi ve bileşenlerini eklemek için kullanılır.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAdditionalUnitOfWork<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : UnitOfWork
    {




        //services.AddInternalUnitOfWorkServices();
        var unitOfWorkOptions = new UnitOfWorkOptions();
        configuration.Bind(UnitOfWorkOptions.UnitOfWorkOptionsSection, unitOfWorkOptions);
        Action<DbContextOptionsBuilder> optionsAction = GetDbContextOptions(unitOfWorkOptions);
        services.AddScoped<TContext, TContext>();
        if (optionsAction != null)
        {
            services.AddDbContextOptions<TContext>((p, b) => optionsAction(b));
        }
        services.AddScoped<IUnitOfWork, TContext>();
        services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
        return services;
    }


    public static IServiceCollection AddAdditionalUnitOfWork<TContext>(this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction = null) where TContext : UnitOfWork
    {
        services.AddInternalUnitOfWorkServices();



        if (optionsAction != null) services.AddDbContextOptions<TContext>((p, b) => optionsAction(b));
        services.AddScoped<TContext, TContext>();
        services.AddScoped<IUnitOfWork<TContext>, UnitOfWork<TContext>>();
        services.AddDbContextFactory<TContext>(lifetime: ServiceLifetime.Scoped);
        return services;
    }




    /// <summary>
    ///     UnitOfWork servisi ve bileşenlerini eklemek için kullanılır.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    //public static IServiceCollection AddUnitOfWork(this IServiceCollection services, Type unitOfworkType)
    //{
    //    if (!typeof(UnitOfWork).IsAssignableFrom(unitOfworkType))
    //    {
    //        throw new Exception("Invalid unitOfWorkType");
    //    }
    //    services.AddInternalUnitOfWorkServices();
    //    services.AddScoped(unitOfworkType);
    //    return services;
    //}
    private static Action<DbContextOptionsBuilder> GetDbContextOptions(UnitOfWorkOptions unitOfWorkOptions)
    {
        if (unitOfWorkOptions?.DatabaseOptions == null)
        {
            return null;
        }
        var dbContextOptionsBuilder = new DbContextOptionsBuilder();
        switch (unitOfWorkOptions.DatabaseOptions.DatabaseType)
        {
            case Juga.Abstractions.Data.Enums.DatabaseType.SqlServer:
                {
                    return (dbContextOptionsBuilder) => { CreateSqlServerDbContextOptions(dbContextOptionsBuilder, unitOfWorkOptions.DatabaseOptions); };
                }
            case Juga.Abstractions.Data.Enums.DatabaseType.PostgreSql:
                {
                    return (dbContextOptionsBuilder) => { CreatePostgreSqlDbContextOptions(dbContextOptionsBuilder, unitOfWorkOptions.DatabaseOptions); };
                }
            default:
                {
                    return (dbContextOptionsBuilder) => { CreateSqlServerDbContextOptions(dbContextOptionsBuilder, unitOfWorkOptions.DatabaseOptions); };
                }
        }
    }
    private static DbContextOptionsBuilder CreateSqlServerDbContextOptions(DbContextOptionsBuilder dbContextOptionsBuilder, DatabaseOptions databaseOptions)
    {
        dbContextOptionsBuilder.UseSqlServer(databaseOptions.ConnectionString);
        var builder = new SqlServerDbContextOptionsBuilder(dbContextOptionsBuilder);
        if (!string.IsNullOrEmpty(databaseOptions.MigrationAssemblyName))
        {
            builder.MigrationsAssembly(databaseOptions.MigrationAssemblyName);
        }
        return dbContextOptionsBuilder;
    }
    private static DbContextOptionsBuilder CreatePostgreSqlDbContextOptions(DbContextOptionsBuilder dbContextOptionsBuilder, DatabaseOptions databaseOptions)
    {
        dbContextOptionsBuilder.UseNpgsql(databaseOptions.ConnectionString);
        var builder = new NpgsqlDbContextOptionsBuilder(dbContextOptionsBuilder);
        if (!string.IsNullOrEmpty(databaseOptions.MigrationAssemblyName))
        {
            builder.MigrationsAssembly(databaseOptions.MigrationAssemblyName);
        }
        return dbContextOptionsBuilder;
    }
    public static IServiceCollection AddInternalUnitOfWorkServices(this IServiceCollection services)
    {

        services.AddSingleton<IAuditPropertyInterceptor, HasCreateDateInterceptor>();
        services.AddSingleton<IAuditPropertyInterceptor, HasUpdateDateInterceptor>();
        services.AddSingleton<IAuditPropertyInterceptor, HasCreatedByInterceptor>();
        services.AddSingleton<IAuditPropertyInterceptor, HasUpdatedByInterceptor>();
        services.AddSingleton<IAuditPropertyInterceptor, HasCreatedAtInterceptor>();
        services.AddSingleton<IAuditPropertyInterceptor, HasUpdatedAtInterceptor>();
        services.AddSingleton<IAuditPropertyInterceptor, HasCreatedByUserCodeInterceptor>();
        services.AddSingleton<IAuditPropertyInterceptor, HasUpdatedByUserCodeInterceptor>();
        services.AddScoped<IAuditLogStore, NullAuditLogStore>();
        services.AddScoped<IAuditEventCreator, NullAuditEventCreator>();
        
        services.AddSingleton<IAuditPropertyInterceptorManager, AuditPropertyInterceptorManager>();
        return services;
    }

    private static void AddDbContextOptions<TContext>(this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> optionsAction) where TContext : UnitOfWork
    {
        services.TryAdd(
            new ServiceDescriptor(
                typeof(DbContextOptions<TContext>),
                p => CreateDbContextOptions<TContext>(p, optionsAction),
                ServiceLifetime.Scoped));

        services.Add(
            new ServiceDescriptor(
                typeof(DbContextOptions),
                p => p.GetRequiredService<DbContextOptions<TContext>>(),
                ServiceLifetime.Scoped));
    }

    private static void AddAdditionalDbContextOptions<TContext>(this IServiceCollection services) where TContext : UnitOfWork
    {
        services.Add(
            new ServiceDescriptor(
                typeof(DbContextOptions),
                p => p.GetRequiredService<DbContextOptions<TContext>>(),
                ServiceLifetime.Scoped));
    }

    private static DbContextOptions<TContext> CreateDbContextOptions<TContext>(
        IServiceProvider applicationServiceProvider,
        Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
        where TContext : UnitOfWork
    {
        var builder = new DbContextOptionsBuilder<TContext>(
            new DbContextOptions<TContext>(new Dictionary<Type, IDbContextOptionsExtension>()));

        builder.UseApplicationServiceProvider(applicationServiceProvider);
        builder.AddInterceptors(
            new AuditableEntityInterceptor(
                applicationServiceProvider.GetRequiredService<IUserContextProvider>()
                , applicationServiceProvider.GetRequiredService<IAuditPropertyInterceptorManager>()
            )
            , new AuditLogInterceptor(
                applicationServiceProvider.GetRequiredService<IOptions<UnitOfWorkOptions>>()
                , applicationServiceProvider.GetRequiredService<IAuditEventCreator>()
                , applicationServiceProvider.GetRequiredService<IAuditLogStore>()
                , applicationServiceProvider.GetRequiredService<IAuditBehaviourService>()
                , applicationServiceProvider.GetRequiredService<IUserContextProvider>()
            )

        );
        var mediator = applicationServiceProvider.GetService<IMediator>();

             if (mediator != null)
        {
            builder.AddInterceptors(new DispatchDomainEventsInterceptor(
                 applicationServiceProvider.GetRequiredService<IMediator>()
            ));
        }

        optionsAction?.Invoke(applicationServiceProvider, builder);

        return builder.Options;
    }
}