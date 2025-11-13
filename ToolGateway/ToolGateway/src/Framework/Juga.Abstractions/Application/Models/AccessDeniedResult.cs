namespace Juga.Abstractions.Application.Models;

/// <summary>
/// Invalid result.
/// </summary>
public class AccessDeniedResult<T> : Result<T>
{
    public AccessDeniedResult(string error = null)
    {
        Messages.Add(error ?? "User is unauthorized.");
    }

    public override ResultType ResultType => ResultType.Unauthorized;

    public override sealed List<string> Messages { get; set; }

    public override T Data => default;
}