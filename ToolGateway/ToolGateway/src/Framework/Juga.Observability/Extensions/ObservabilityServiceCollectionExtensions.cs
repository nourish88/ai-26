using Juga.Observability.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Juga.Observability.Extensions
{
    /// <summary>
    /// Observability alt yapı bileşeninin uygulama seviyesinde konfigurasyonu için kullanılır.
    /// </summary>
    public static class ObservabilityServiceCollectionExtensions
    {
        public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
        {

            ObservabilityOptions observabilityOptions = new();
            var section = configuration.GetSection(ObservabilityOptions.ObservabilityOptionsSection);

            if (section.Exists())
            {
                section.Bind(observabilityOptions);
            }
            else
            {
                // Handle the case when the section is not found
                Console.WriteLine($"Section '{ObservabilityOptions.ObservabilityOptionsSection}' not found in configuration.");
                return services;
            }

            if (!observabilityOptions.Enabled)
            {
                return services;
            }
            services
                .AddOpenTelemetry()
                .ConfigureResource(resource => 
                    resource.AddService(
                        serviceName: observabilityOptions.ServiceName,
                        serviceVersion: observabilityOptions.ServiceVersion,
                        serviceInstanceId: Environment.MachineName
                    )
                    .AddTelemetrySdk()
                )                    
                .AddMetrics(observabilityOptions)
                .AddTracing(observabilityOptions);

            return services;
        }
        private static OpenTelemetryBuilder AddTracing(this OpenTelemetryBuilder builder, ObservabilityOptions observabilityOptions)
        {
            if (!observabilityOptions.EnabledTracing) return builder;

            builder.WithTracing(tracing =>
            {
                tracing
                    .SetErrorStatusOnException()
                    .SetSampler(new AlwaysOnSampler())
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.Filter = (context) =>
                        {
                            if (!string.IsNullOrEmpty(context.Request.Path.Value))
                            {
                                if (context.Request.Path.Value.StartsWith("/swagger")) return false;
                                return context.Request.Path.Value.Contains('/', StringComparison.InvariantCulture);
                            }
                            return false;
                        };
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.SetTag("requestProtocol", httpRequest.Protocol);
                        };
                        options.EnrichWithHttpResponse = (activity, httpResponse) =>
                        {
                            activity.SetTag("responseLength", httpResponse.ContentLength);
                        };
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exceptionType", exception.GetType().ToString());
                        };
                    })
                    .AddEntityFrameworkCoreInstrumentation(efcoreOptions =>
                    {
                        efcoreOptions.SetDbStatementForText = true;
                        efcoreOptions.SetDbStatementForStoredProcedure = true;
                    })
                    .AddHttpClientInstrumentation(cfg =>
                    {
                        cfg.RecordException = true;

                    })
                    .AddSource("MassTransit")
                    .AddConsoleExporter()
                    .AddOtlpExporter(_ =>
                    {
                        _.Endpoint = observabilityOptions.CollectorUri;
                        _.ExportProcessorType = ExportProcessorType.Batch;
                        _.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    });
            });

            return builder;
        }

        private static OpenTelemetryBuilder AddMetrics(this OpenTelemetryBuilder builder, ObservabilityOptions observabilityOptions)
        {
            if (!observabilityOptions.EnabledMetrics) return builder;
            builder.WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddMeter("MassTransit")
                    .AddConsoleExporter()
                    .AddOtlpExporter(_ =>
                    {
                        _.Endpoint = observabilityOptions.CollectorUri;
                        _.ExportProcessorType = ExportProcessorType.Batch;
                        _.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    });
            });

            return builder;
        }
    }
}
