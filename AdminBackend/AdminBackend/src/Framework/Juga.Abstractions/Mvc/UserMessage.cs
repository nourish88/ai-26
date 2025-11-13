namespace Juga.Abstractions.Mvc
{
    public class UserMessage
    {
        public string Message { get; private set; }
        public string Status { get; private set; }

        public UserMessage(string message, string status)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            Message = message;
            Status = status;
        }
    }
}