namespace Juga.Abstractions.Mvc
{
    public interface IUserMessageService
    {
        void SetMessage(UserMessage userMessage);

        UserMessage GetMessage();
    }
}