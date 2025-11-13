namespace Juga.MessageQueue.Enums;

/// <summary>
/// Kullanılan masstransit kütüphanesi çeşitli kuyruk mekanizmalarını desteklemektedir.
/// </summary>
public enum QueueType
{
    None = 0,
    RabbitMQ = 1,
    InMemory=2,
}