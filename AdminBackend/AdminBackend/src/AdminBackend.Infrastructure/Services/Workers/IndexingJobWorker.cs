using System.Threading.Channels;
using AdminBackend.Application.Features.Ingestion;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdminBackend.Infrastructure.Services.Workers;

public class IndexingJobWorker(
    ILogger<IndexingJobWorker> logger,
    Channel<IndexFileCommand> channel, 
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Indexing job worker initialized..");
        try
        {
            await foreach (var command in channel.Reader.ReadAllAsync(stoppingToken))
            {
                using var scope = scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                logger.LogInformation("Indexing job received for file: {FileId}", command.File.Id);
                var result = await sender.Send(command, stoppingToken);
                if (result.Success)
                {
                    logger.LogInformation("Indexing job completed for file: {FileId}", command.File.Id);
                }
                else
                {
                    logger.LogError("Indexing job has failed for file: {FileId}", command.File.Id);
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Indexing job worker stopped");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while processing indexing job");;        
        }
    }
}