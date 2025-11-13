using Juga.Client.Abstractions;
using Juga.Client.Extensions;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace Juga.Client.Providers;

public class HttpClientProvider(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider,
        IWebHostEnvironment env)
    : IHttpClientProvider
{
    IWebHostEnvironment _env = env;

    //public async Task<T> ExecuteAsync<TService, T>(Func<TService, Task<T>> func)
    //{
    //    return await GetAsync<T>(func.Method.Name, func.Method.GetParameters().GetValue(0));
    //}

    /// <summary>
    /// For getting a single item from a web api uaing GET
    /// </summary>
    /// <param name="apiUrl">Added to the base address to make the full url of the 
    ///     api get method, e.g. "products/1" to get a product with an id of 1</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The item requested</returns>
    public async Task<T> GetAsync<T>(string clientName, string apiUrl, object parameter = null, CancellationToken cancellationToken = default)
    {
        if (parameter != null)
            apiUrl = apiUrl.AddQueryString(parameter);
        var client = await GetHttpClient(clientName);
        string json;
        try
        {
            json = await client.GetStringAsync(apiUrl, cancellationToken);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return default(T);
        }
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        var model = JsonSerializer.Deserialize<T>(json, options);
        return model;
    }

    /// <summary>
    /// For creating a new item over a web api using POST
    /// </summary>
    /// <param name="apiUrl">Added to the base address to make the full url of the 
    ///     api post method, e.g. "products" to add products</param>
    /// <param name="postObject">The object to be created</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The item created</returns>
    public async Task<TResult> PostAsync<T, TResult>(string clientName, string apiUrl, T postObject, CancellationToken cancellationToken)
    {
        var client = await GetHttpClient(clientName);
        var result = default(TResult);
        var payload = JsonSerializer.Serialize(postObject);

        var response = await client.PostAsync(apiUrl, new StringContent(payload, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            await response.Content.ReadAsStringAsync(cancellationToken).ContinueWith((Task<string> x) =>
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                result = JsonSerializer.Deserialize<TResult>(x.Result, options);
            }, cancellationToken);
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            response.Content?.Dispose();
            throw new HttpRequestException($"{response.StatusCode}:{content}");
        }
        return result;
    }

    /// <summary>
    /// For updating an existing item over a web api using PUT
    /// </summary>
    /// <param name="apiUrl">Added to the base address to make the full url of the 
    ///     api put method, e.g. "products/3" to update product with id of 3</param>
    /// <param name="putObject">The object to be edited</param>
    /// <param name="cancellationToken"></param>
    public async Task PutAsync<T>(string clientName, string apiUrl, T putObject, CancellationToken cancellationToken)
    {
        var client = await GetHttpClient(clientName);
        var payload = JsonSerializer.Serialize(putObject);
        var response = await client.PutAsync(apiUrl, new StringContent(payload, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            response.Content?.Dispose();
            throw new HttpRequestException($"{response.StatusCode}:{content}");
        }
    }

    /// <summary>
    /// For deleting an existing item over a web api using DELETE
    /// </summary>
    /// <param name="apiUrl">Added to the base address to make the full url of the 
    ///     api delete method, e.g. "products/3" to delete product with id of 3</param>
    /// <param name="cancellationToken"></param>
    public async Task DeleteAsync(string clientName, string apiUrl, CancellationToken cancellationToken)
    {
        var client = await GetHttpClient(clientName);
        var response = await client.DeleteAsync(apiUrl, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            response.Content?.Dispose();
            throw new HttpRequestException($"{response.StatusCode}:{content}");
        }
    }

    private async Task<HttpClient> GetHttpClient(string clientName)
    {

        var accessToken = await tokenProvider.GetToken(TokenType.AccessToken);
        var client = httpClientFactory.CreateClient(clientName);
            
        if (!string.IsNullOrEmpty(accessToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }
}