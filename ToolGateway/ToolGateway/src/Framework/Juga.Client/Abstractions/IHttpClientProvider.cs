namespace Juga.Client.Abstractions;

public interface IHttpClientProvider
{
    //Task<T> ExecuteAsync<TService, T>(Func<TService, Task<T>> func);
    Task<T> GetAsync<T>(string clientName, string apiUrl, object parameter = null, CancellationToken token = default);
    //Task<T[]> GetMultipleItemsRequest<T>(string apiUrl, CancellationToken token = default);
    Task<TResult> PostAsync<T, TResult>(string clientName, string apiUrl, T postObject, CancellationToken token = default);
    Task PutAsync<T>(string clientName, string apiUrl, T putObject, CancellationToken token = default);
    Task DeleteAsync(string clientName, string apiUrl, CancellationToken token = default);
}