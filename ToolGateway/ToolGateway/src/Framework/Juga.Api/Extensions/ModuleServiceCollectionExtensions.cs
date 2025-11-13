
namespace Juga.Api.Extensions;

public static class ModuleServiceCollectionExtensions
{
    public static IServiceCollection AddModule<TContext>(this IServiceCollection services,
        IConfiguration configuration, ApiOptions options
        , Action<DbContextOptionsBuilder> dbContextoptionsAction = null)
        where TContext : UnitOfWork
    {

        services.ConfigureUnitOfWork(configuration);
   
        if (dbContextoptionsAction != null)
        {
            services.AddUnitOfWork<TContext>(dbContextoptionsAction);
        }
        else
        {
            services.AddUnitOfWork<TContext>(configuration);
        }


        services.ConfigureAuditLogs(configuration, options);


       

        return services;
    }
}

