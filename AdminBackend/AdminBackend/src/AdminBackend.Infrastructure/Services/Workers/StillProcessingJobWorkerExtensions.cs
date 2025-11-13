using Microsoft.Extensions.DependencyInjection;

namespace AdminBackend.Infrastructure.Services.Workers;

public static class StillProcessingJobWorkerExtensions
{
    public static IServiceCollection AddStillProcessingJobWorker(this IServiceCollection services)
    {
        services.AddHostedService<StillProcessingJobWorker>();
        return services;
    }
}