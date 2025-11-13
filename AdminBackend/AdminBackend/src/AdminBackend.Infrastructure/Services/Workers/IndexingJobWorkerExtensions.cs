using System.Threading.Channels;
using AdminBackend.Application.Features.Ingestion;
using Microsoft.Extensions.DependencyInjection;

namespace AdminBackend.Infrastructure.Services.Workers;

public static class IndexingJobWorkerExtensions
{
    public static IServiceCollection AddIndexingJobWorker(this IServiceCollection services)
    {
        services.AddHostedService<IndexingJobWorker>();
        services.AddSingleton<Channel<IndexFileCommand>>(_ => 
            Channel.CreateBounded<IndexFileCommand>(new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.Wait,
                AllowSynchronousContinuations = false,
                SingleReader = true,
                SingleWriter = false,
            }));

        return services;
    }
}