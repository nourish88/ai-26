using Juga.MessageQueue.Enums;

namespace Juga.MessageQueue.Extensions
{
    public class MessageQueueOptions
    {
        public const string OptionsSection = "Juga:Queue";

        public bool IsEnabled { get; set; } = false;
        public QueueType QueueType { get; set; } = QueueType.None;
        public OutboxType OutboxType { get; set; } = OutboxType.None;
        public ConnectionStrings ConnectionStrings { get; set; }
        public string RabbitMQUserName { get; set; } = "guest";
        public string RabbitMQPassword { get; set; } = "guest";
    }

    public class ConnectionStrings
    {
        public string RabbitMQ { get; set; }
    }
}
