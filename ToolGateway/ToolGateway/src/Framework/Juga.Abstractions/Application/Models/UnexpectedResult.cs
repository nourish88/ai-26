namespace Juga.Abstractions.Application.Models;

/// <summary>
///     Unexpected result.
/// </summary>
public class UnexpectedResult<T> : Result<T>
{
    public UnexpectedResult(string error = null)
    {
        Messages.Add(error ?? "There was an unexpected problem");
    }

    public UnexpectedResult()
    {
    }

    public override ResultType ResultType => ResultType.Unexpected;

    public override sealed List<string> Messages { get; set; }

    public override T Data => default;
}