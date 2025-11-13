using Juga.Abstractions.Mvc;

namespace Juga.Mvc;

public class UserMessageService : IUserMessageService
{
    private UserMessage UserMessage { get; set; }

    public UserMessage GetMessage()
    {
        if (UserMessage != null)
        {
            var message = new UserMessage(UserMessage.Message.Clone().ToString(),
                UserMessage.Status.Clone().ToString());
            UserMessage = null;
            return message;
        }

        return null;
    }

    public void SetMessage(UserMessage userMessage)
    {
        UserMessage = userMessage;
    }
}