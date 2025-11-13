using Juga.MessageQueue.Enums;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;



namespace Juga.MessageQueue.Configurations;

public class RabbitMqSettings
{
    public const string Section = "Juga:RabbitMq";
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public record MassTransitRegistrationConfigs(
    IEnumerable<Assembly> Assemblies,
    QueueType QueueType,
    RabbitMqSettings RabbitMqSettings,
    IBusRegistrationConfigurator BusRegistrationConfigurator,
    IConfigurationSection QueueConfigSection);
public record MassTransitConfigs(
    IServiceCollection Services,
    IEnumerable<Assembly> Assemblies,
    IConfiguration Configuration,
    QueueType QueueType,
    RabbitMqSettings RabbitMqSettings);
