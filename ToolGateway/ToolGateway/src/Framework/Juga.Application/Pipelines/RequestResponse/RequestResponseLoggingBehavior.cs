using System.Text.Json;
using Serilog;

namespace Juga.Application.Pipelines.RequestResponse;

public class RequestResponseLoggingBehavior<TRequest, TResponse>
     : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>, IRRLoggingRequest

{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Encoder =System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        // Request Logging
        // Serialize the request
        var requestJson = JsonSerializer.Serialize(request, options);
        // Log the serialized request
        Log.Information("Handling request {CorrelationID}: {Request}", correlationId, requestJson);

        // Response logging
        var response = await next();
        // Serialize the request
        var responseJson = JsonSerializer.Serialize(response, options);
        // Log the serialized request
        Log.Information("Response for {Correlation}: {Response}", correlationId, responseJson);

        // Return response
        return response;
    }
}
