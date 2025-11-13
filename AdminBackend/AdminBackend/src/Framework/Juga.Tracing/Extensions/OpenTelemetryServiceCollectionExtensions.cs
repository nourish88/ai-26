using Juga.Tracing.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Juga.Tracing.Extensions;

public static class OpenTelemetryServiceCollectionExtensions
{
    public static void TryAddTracing(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("Juga:OpenTelemetry:IsEnabled"))
        {
            var openTelemetryConstants = (configuration.GetSection("Juga:OpenTelemetry").Get<OpenTelemetryConstants>())!;
            ActivitySourceProvider.Source = new System.Diagnostics.ActivitySource(openTelemetryConstants.ActivitySourceName);
            services.AddOpenTelemetry()
                .WithTracing(options =>
                {
                    options
                        .AddSource(openTelemetryConstants.ActivitySourceName)
                        .ConfigureResource(resource =>
                        {
                            resource.AddService(openTelemetryConstants.ServiceName, serviceVersion: openTelemetryConstants.ServiceVersion);
                        });
                    options.AddAspNetCoreInstrumentation(aspnetcoreOptions =>
                    {
                        aspnetcoreOptions.Filter = (context) =>
                        {
                            if (!string.IsNullOrEmpty(context.Request.Path.Value))
                            {
                                if (context.Request.Path.Value.StartsWith("/swagger")) return false;
                                return context.Request.Path.Value.Contains("/", StringComparison.InvariantCulture);
                            }
                            return false;
                        };
                        aspnetcoreOptions.RecordException = true;
                        aspnetcoreOptions.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.SetTag("requestProtocol", httpRequest.Protocol);
                        };
                        aspnetcoreOptions.EnrichWithHttpResponse = (activity, httpResponse) =>
                        {
                            activity.SetTag("responseLength", httpResponse.ContentLength);
                        };
                        aspnetcoreOptions.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exceptionType", exception.GetType().ToString());
                        };
                    });
                    options.AddEntityFrameworkCoreInstrumentation(efcoreOptions =>
                    {
                        efcoreOptions.SetDbStatementForText = true;
                        efcoreOptions.SetDbStatementForStoredProcedure = true;
                    });
                    options.AddHttpClientInstrumentation();
                    options.AddConsoleExporter();
                    //options.AddOtlpExporter(options => options.Endpoint = new Uri(openTelemetryConstants.ExporterUri));
                    options.AddOtlpExporter(); // Push data to Jaeger
                });
        }
    }
}