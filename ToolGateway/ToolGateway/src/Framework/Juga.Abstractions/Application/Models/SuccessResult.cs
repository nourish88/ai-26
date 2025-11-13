namespace Juga.Abstractions.Application.Models;

/// <summary>
///     Success result.sssss
/// </summary>
public class SuccessResult<T> : Result<T>
{
    private readonly T _data;

    public SuccessResult(T data)
    {
        _data = data;
    }

    public SuccessResult(T data, string[] messages)
    {
        _data = data;

        Messages.AddRange(messages);
    }

    public SuccessResult(T data, string message)
    {
        _data = data;
        Messages.Add(message);
    }

    public override ResultType ResultType => ResultType.Ok;

    public override sealed List<string> Messages { get; set; }

    public override T Data => _data;
}