using Juga.Abstractions.Messaging;

namespace Juga.MessageQueue.Services;

public interface IQueueService
{
    Task Send<T>(T message) where T : IQueueCommandMessage;




    /// <summary>
    /// 'queueName' isimli kuyruğa verilen mesajı gönderir.
    /// </summary>
    /// <param name="queueName">Mesaj gönderilecek kuyruk</param>
    /// <param name="message">Mesaj</param>
    /// <returns></returns>
    Task Send(string queueName, IQueueCommandMessage message);

    /// <summary>
    /// Event publish etmek için kullanılır. Bu event ile ilgilenen tüm consumerlar tarafından tüketilebilir.
    /// </summary>
    /// <param name="publishEvent">Publish edilecek event</param>
    /// <returns></returns>
    Task Publish(IEvent publishEvent);
}