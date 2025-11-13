using Juga.Abstractions.Attributes.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Juga.Logging.Serilog.Middlewares;


// Mediatr Yoksa Aktif Olur
public class RequestResponseLoggingMiddleware(RequestDelegate next, IDiagnosticContext diagnosticContext, ILogger<RequestResponseLoggingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {

        var endpoint = context.GetEndpoint();
        if (endpoint != null)
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
        var ignoreAttribute = endpoint?.Metadata.GetMetadata<IgnoreRRLogging>();
          if (ignoreAttribute != null) 
        {
            await next(context);
            return;
        }
        



            diagnosticContext.Set("Host", context.Request.Host);
        diagnosticContext.Set("Protocol", context.Request.Protocol);
        diagnosticContext.Set("Scheme", context.Request.Scheme);

        if (context.Request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", context.Request.QueryString.Value);
        }

        var requestBodyPayload = await ReadRequestBody(context.Request);
        diagnosticContext.Set("RequestBody", requestBodyPayload);

        

        var request = context.Request;
        var requestBody = "";
        if (request.ContentLength > 0)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            requestBody = await reader.ReadToEndAsync();


            request.Body.Seek(0, SeekOrigin.Begin);
        }
        var ep = endpoint?.DisplayName ?? "";
        var routeValues = new StringBuilder();
        foreach (var routeValue in request.RouteValues)
        {
            routeValues.Append(routeValue.Key);
            routeValues.Append(" : ");
            routeValues.Append(routeValue.Value);
            routeValues.Append(" , ");
        }

        var guid = Guid.NewGuid();
        //_logger.LogInformation("Request: {ep} {Host} {Protocol} {Scheme} {Method} {Path} {QueryString} {RequestBody} ", ep, request.Host, request.Protocol, request.Scheme,request.Method, request.Path, request.QueryString.Value, requestBody);
        logger.LogInformation($"RequestId:{guid},   Request Log: EndPoint={ep}, Host:{request.Host}, Protocol: {request.Protocol}, Scheme: {request.Scheme}, Path:{request.Path}, QueryString:{request.QueryString.Value}, Request Body: {requestBody}, Route Values =  {routeValues.ToString()}");
        var response = context.Response;

        var durationAttribute = endpoint?.Metadata.GetMetadata<DurationAttribute>();
        if (durationAttribute != null)
            await AddDurationToPayloadAndLogResponse(context, response);

        else
            await LogResponse(context, response,guid);







    }

    private async Task LogResponse(HttpContext context, HttpResponse response, Guid guid)
    {
        using var responseBody = new MemoryStream();
        var originalResponseBodyStream = context.Response.Body;
        context.Response.Body = responseBody;
        await next(context);
        string responseBodyPayload = await ReadResponseBody(context.Response);
        diagnosticContext.Set("ResponseBody", responseBodyPayload);
        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalResponseBodyStream);
        logger.LogInformation($"RequestId={guid},  Response Log: Status Code= {response.StatusCode}, Response Content= {responseContent}");
    }

    private async Task AddDurationToPayloadAndLogResponse(HttpContext context, HttpResponse response)
    {
        var stopwatch = Stopwatch.StartNew();
        using var responseBody = new MemoryStream();
        var originalResponseBodyStream = context.Response.Body;
        context.Response.Body = responseBody;
        await next(context);
        stopwatch.Stop();
        var duration = stopwatch.ElapsedMilliseconds;
        string responseBodyPayload = await ReadResponseBody(context.Response);
        var responseObject = JsonConvert.DeserializeObject<JObject>(responseBodyPayload);


        responseObject["duration"] = duration;

        var updatedResponse = JsonConvert.SerializeObject(responseObject);

        // Convert the updated response content back to bytes
        var updatedResponseBytes = Encoding.UTF8.GetBytes(updatedResponse);

        // Write the updated response to the original response stream

        await originalResponseBodyStream.WriteAsync(updatedResponseBytes);
        logger.LogInformation($"Response Log: Status Code= {response.StatusCode}, Response Content= {updatedResponse}");
    }

    private void LogResponse(HttpResponse response, string responseContent)
    {
        logger.LogInformation($"Response Log: Status Code= {response.StatusCode}, Response Content= {responseContent}");
    }

  

    private async Task<string> ReadRequestBody(HttpRequest request)
    {
        HttpRequestRewindExtensions.EnableBuffering(request);

        var body = request.Body;
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        string requestBody = Encoding.UTF8.GetString(buffer);
        body.Seek(0, SeekOrigin.Begin);
        request.Body = body;

        return $"{requestBody}";
    }

    private static async Task<string> ReadResponseBody(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return $"{responseBody}";
    }
}