using AdminBackend.Application.Features.Ingestion;
using AdminBackend.Application.Settings;
using AdminBackend.Domain.Constants;
using Juga.Data.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using File = AdminBackend.Domain.Entities.File;

namespace AdminBackend.Infrastructure.Services.Workers;

public class StillProcessingJobWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<StillProcessingJobWorkerSettings> workerSettings,
    ILogger<StillProcessingJobWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Still processing job worker initialized..");
        
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(workerSettings.Value.CheckIntervalInMins));
        
        do
        {
            using var serviceScope = scopeFactory.CreateScope();
            var fileRepository = serviceScope.ServiceProvider.GetRequiredService<IRepository<File>>();
            var sender = serviceScope.ServiceProvider.GetRequiredService<ISender>();

            try
            {
                var entities = await fileRepository
                    .Where(x =>
                        x.IngestionStatusTypeId == IngestionStatusTypes.ProcessingRequested &&
                        x.CreatedDate < DateTime.UtcNow.AddMinutes(-1 * workerSettings.Value.MaxProcessIntervalInMins))
                    .ToListAsync(stoppingToken);

                logger.LogInformation("Found {EntityCount} entitie(s) to process", entities.Count);

                if (entities.Count != 0)
                {
                    foreach (var file in entities)
                    {
                        await sender.Send(new SendJobRequestCommand(file), stoppingToken);
                    }
                    logger.LogInformation("All entities sent to processing pipeline");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing for still processing files");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}