namespace Juga.Abstractions.ExceptionHandling;

/// <summary>
/// Herhangi bir exception durumunda apiden dönecek olan nesne
/// </summary>
//TODO: Juga.Api.Application projesinde Models klasöründe api return sınıfları var kontrol edilmeli.
public class ErrorResult
{
    public int StatusCode { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public string StackTrace { get; set; }
    public string Source { get; set; }
}