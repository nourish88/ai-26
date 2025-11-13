namespace Juga.Abstractions.Application.Models;

/// <summary>
///     Invalid result.
/// </summary>
public class InvalidResult<T> : Result<T> where T : new()
{
    public InvalidResult(string error)
    {
        Messages.Add(error ?? "The input was invalid.");
    }

    public InvalidResult(string[] messages)
    {
        Messages.AddRange(messages);
    }

    public override ResultType ResultType => ResultType.Invalid;

    public override sealed List<string> Messages { get; set; }

    public override T Data => new();
}