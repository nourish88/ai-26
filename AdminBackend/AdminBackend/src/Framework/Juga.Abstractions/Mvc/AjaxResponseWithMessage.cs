namespace Juga.Abstractions.Mvc
{
    public class AjaxResponseWithMessage : UserMessage
    {
        public AjaxResponseWithMessage(string message, string status) : base(message, status)
        {
        }
    }

    public class AjaxResponseWithMessage<T> : UserMessage
    {
        public T Data { get; set; }

        public AjaxResponseWithMessage(T data, string message, string status) : base(message, status)
        {
            Data = data;
        }
    }
}