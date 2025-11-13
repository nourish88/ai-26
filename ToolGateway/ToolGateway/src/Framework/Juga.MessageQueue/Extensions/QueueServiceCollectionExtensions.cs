using Juga.MessageQueue.Configurations;
using Juga.MessageQueue.Enums;
using Juga.MessageQueue.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Juga.MessageQueue.Extensions;

public static class QueueServiceCollectionExtensions
{
    /// <summary>
    /// Konfigurasyonda yer alan kuyruk bilgileri doğrultusunda gerekli servisleri ayağa kaldırır.
    /// Verilen assembly listesini tarar ve bu assemblylerde yer alan tüm consumerları register eder.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="assemblies">Assembly listesi</param>
    /// <returns></returns>
    public static IServiceCollection TryAddQueueServices<TContext>(this IServiceCollection services,
        IConfiguration configuration,
        IEnumerable<Assembly> assemblies) where TContext : DbContext
    {
        var queueConfigSection = configuration.GetSection("Juga:Queue");
        var queueType = queueConfigSection.GetValue<QueueType>("QueueType");
        var isConfigNull = queueConfigSection == null;
        var isNotEnabled = !queueConfigSection.GetValue<bool>("IsEnabled");
        var isNoneType = queueType == QueueType.None;

        if (isConfigNull || isNotEnabled || isNoneType) return services;
        var rabbitMqSettings = BindRabbitMqSettings(services, configuration);
        services.AddScoped<IQueueService, QueueService>();
        ConfigureMasstransit<TContext>(new MassTransitConfigs(services, assemblies, configuration, queueType, rabbitMqSettings));
        return services;
    }



    public static IServiceCollection TryAddQueueServices(this IServiceCollection services,
        IConfiguration configuration,
        IEnumerable<Assembly> assemblies)
    {
     

        var queueConfigSection = configuration.GetSection("Juga:Queue");       
        var queueType = queueConfigSection.GetValue<QueueType>("QueueType");
        var isConfigNull = queueConfigSection == null;
        var isNotEnabled = !queueConfigSection.GetValue<bool>("IsEnabled");
        var isNoneType = queueType == QueueType.None;

        if (isConfigNull || isNotEnabled || isNoneType) return services;
        var rabbitMqSettings = BindRabbitMqSettings(services, configuration);
        services.AddScoped<IQueueService, QueueService>();
        ConfigureMasstransit(new MassTransitConfigs(services, assemblies, configuration, queueType, rabbitMqSettings));


        return services;
    }

    private static RabbitMqSettings BindRabbitMqSettings(IServiceCollection services, IConfiguration configuration)
    {
        var rabbitMqSettings = new RabbitMqSettings();
        configuration.GetSection(RabbitMqSettings.Section).Bind(rabbitMqSettings);
        services.AddSingleton(rabbitMqSettings);
        return rabbitMqSettings;
    }

    private static void ConfigureMasstransit<TContext>(MassTransitConfigs configs) where TContext : DbContext
    {
        var queueConfigSection = configs.Configuration.GetSection("Juga:Queue");
        var outboxType = queueConfigSection.GetValue<OutboxType>("OutboxType");
        configs.Services.AddMassTransit(x =>
        {
            AddMassTransit(new MassTransitRegistrationConfigs(configs.Assemblies, configs.QueueType, configs.RabbitMqSettings, x, queueConfigSection));

            AddOutbox<TContext>(x, outboxType);
            AddSagaRepositoryProvider<TContext>(x);
        });
        AddOptions(configs.Services);
    }   

    private static void ConfigureMasstransit(MassTransitConfigs configs)
    {
        var queueConfigSection = configs.Configuration.GetSection("Juga:Queue");
        // var outboxType = queueConfigSection.GetValue<OutboxType>("OutboxType");
        configs.Services.AddMassTransit(x =>
        {
            AddMassTransit(new MassTransitRegistrationConfigs(configs.Assemblies, configs.QueueType, configs.RabbitMqSettings, x, queueConfigSection));            
        });
        
        AddOptions(configs.Services);
    }

    private static void AddOptions(IServiceCollection services)
    {
        services.AddOptions<MassTransitHostOptions>()
            .Configure(options =>
            {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(30);
                options.StopTimeout = TimeSpan.FromSeconds(30);
            });
    }

    private static void AddMassTransit(MassTransitRegistrationConfigs configs)
    {
        var asb = configs.Assemblies.ToArray();
        configs.BusRegistrationConfigurator.SetKebabCaseEndpointNameFormatter();
        //configs.BusRegistrationConfigurator.SetInMemorySagaRepositoryProvider();        
        configs.BusRegistrationConfigurator.AddConsumers(asb);
        configs.BusRegistrationConfigurator.AddSagaStateMachines(asb);
        configs.BusRegistrationConfigurator.AddSagas(asb);
        configs.BusRegistrationConfigurator.AddActivities(asb);
        switch (configs.QueueType)
        {
            case QueueType.RabbitMQ:
                UseRabbitMq(configs);
                break;
            case QueueType.InMemory:
                UseInMemory(configs);
                break;
            case QueueType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(configs.QueueType), configs.QueueType, null);
        }
    }

    private static void UseInMemory(MassTransitRegistrationConfigs configs)
    {
        configs.BusRegistrationConfigurator.UsingInMemory((context, cfg) =>
        {

            cfg.ConfigureEndpoints(context);
        });
    }

    private static void UseRabbitMq(MassTransitRegistrationConfigs configs)
    {
        configs.BusRegistrationConfigurator.UsingRabbitMq((context, cfg) =>
        {

            var connectionString =configs.QueueConfigSection.GetConnectionString("RabbitMQ");
            cfg.Host(new Uri(connectionString), host =>
            {
                host.Username(configs.RabbitMqSettings.Username);
                host.Password(configs.RabbitMqSettings.Password);
            });

            cfg.ConfigureEndpoints(context);
        });
    }

    private static void AddOutbox<TContext>(IBusRegistrationConfigurator x, OutboxType outboxType) where TContext : DbContext
    {
        if (outboxType != OutboxType.None)
        {
            x.AddEntityFrameworkOutbox<TContext>(o =>
            {
                o.QueryDelay = TimeSpan.Zero;
                switch (outboxType)
                {
                    case OutboxType.SqlServer:
                        o.UseSqlServer();
                        break;
                    case OutboxType.Postgres:
                        o.UsePostgres();
                        break;
                    case OutboxType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(outboxType), outboxType, null);
                }
                o.UseBusOutbox();                
                o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
            });
        }
    }

    private static void AddSagaRepositoryProvider<TContext>(IBusRegistrationConfigurator x) where TContext : DbContext
    {
        x.SetEntityFrameworkSagaRepositoryProvider(r =>
        {
            r.ExistingDbContext<TContext>();
            r.UseSqlServer();
        });
    }

}