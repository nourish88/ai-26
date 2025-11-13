using Juga.Abstractions.Messaging;
using Juga.Abstractions.Messaging.Extensions;
using MassTransit;

namespace Juga.MessageQueue.Services;

public class QueueService() : IQueueService
{
    public async Task Publish(IEvent publishEvent)
    {
        //await publisher.Publish((object)publishEvent);
    }

    /// <summary>
    /// Verilen kuyruğa ilgili mesaj iletilir.
    /// </summary>
    /// <param name="queueName">Mesajın gönderileceği kuyruk ismi</param>
    /// <param name="message">Mesaj</param>
    /// <returns></returns>
    public async Task Send(string queueName, IQueueCommandMessage message)
    {
        //var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{queueName}"));
        //await endpoint.Send((object)message);
    }
    public async Task Send<T>(T message) where T : IQueueCommandMessage

    {
        //var queueName = message.GetType().Name.PascalToKebabCase();
        //var endpoint = await bus.GetSendEndpoint(new Uri($"queue:{queueName}"));
        //await endpoint.Send(message);
    }
}