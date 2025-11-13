namespace Juga.Abstractions.Application.Models;

/// <summary>
/// Not found result.
/// </summary>
public class NotFoundResult<T> : Result<T>
{
    public NotFoundResult(string error = null) : base()
    {
        Messages.Add(error ?? "Data not found.");
    }

    public override ResultType ResultType => ResultType.NotFound;

    public override sealed List<string> Messages { get; set; }

    public override T Data => default(T);
}