using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Juga.MessageQueue.Extensions
{
    public static class BusExtensions
    {
        public static async Task Send<T>(this IBus bus, T message, string queueName) where T : class
        {
            var sendEndpoint = await bus.GetSendEndpoint(new Uri($"queue:{queueName}"));
            await sendEndpoint.Send<T>(message);
        }

        public static async Task SendByEndpoint<T>(this IBus bus, T message) where T : class
        {
            var sendEndpoint = await bus.GetPublishSendEndpoint<T>();
            await sendEndpoint.Send<T>(message);
        }
    }
}
