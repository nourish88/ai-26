using System.ComponentModel.DataAnnotations;

namespace Juga.Abstractions.Application.Models;

/// <summary>
///     Result model to contain data, result type, and errors
/// </summary>
public class Result<T>
{
    //check
    public Result()
    {
        Messages = new List<string>();
    }

    public Result(T data, bool durationEnabled, string message = "İşlem başarılı",
        ResultType resultType = ResultType.Ok)
    {
        Messages = new List<string>();
        Data = data;
        Messages.Add(message);
        ResultType = resultType;
    }

    public Result(T data, string message = "İşlem başarılı", ResultType resultType = ResultType.Ok)
    {
        Messages = new List<string>();
        Data = data;
        Messages.Add(message);
        ResultType = resultType;
    }

    public Result(ResultType resultType = ResultType.Ok)
    {
        ResultType = resultType;
    }

    public Result(string message, ResultType resultType = ResultType.Ok)
    {
        Messages = new List<string>();

        Messages.Add(message);
        ResultType = resultType;
    }

    public virtual ResultType ResultType { get; set; }

    public virtual List<string> Messages { get; set; }

    [Required(AllowEmptyStrings = true)] public virtual T Data { get; set; }
}