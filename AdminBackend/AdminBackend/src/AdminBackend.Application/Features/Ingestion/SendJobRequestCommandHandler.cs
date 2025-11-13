using AdminBackend.Application.Repositories;
using AdminBackend.Application.Services.DataManager;
using AdminBackend.Application.Services.FileStorage;
using AdminBackend.Application.Settings;
using AdminBackend.Domain.Entities;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using File = AdminBackend.Domain.Entities.File;

namespace AdminBackend.Application.Features.Ingestion;

public record SendJobRequestCommand(File File) : ICommand;

public class SendJobRequestCommandHandler(
    IMemoryCache memoryCache,
    IFileStorage fileStorage,
    IApplicationFileStoreRepository applicationFileStoreRepository,
    IRepository<ApplicationChunkingStrategy> applicationChunkingStrategyRepository,
    IRepository<ApplicationExtractorEngine> applicationExtractorEngineRepository,
    IOptions<FileStorageSettings> fileStorageSettings,
    IDataManagerService dataManagerService) : ICommandHandler<SendJobRequestCommand>
{
    private async Task<ApplicationChunkingStrategy?> GetCachedApplicationChunkingStrategy(long applicationId,
        CancellationToken cancellationToken)
    {
        return await memoryCache.GetOrCreateAsync($"AppChunkingStrategy-{applicationId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await applicationChunkingStrategyRepository
                .Where(x => x.ApplicationId == applicationId)
                .AsNoTracking()
                .Include(x => x.ChunkingStrategy)
                .FirstOrDefaultAsync(cancellationToken);
        });
    }

    private async Task<ApplicationExtractorEngine?> GetCachedApplicationExtractionEngine(long applicationId,
        CancellationToken cancellationToken)
    {
        return await memoryCache.GetOrCreateAsync($"AppExtractionEngine-{applicationId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await applicationExtractorEngineRepository
                .Where(x => x.ApplicationId == applicationId)
                .AsNoTracking()
                .Include(x => x.ExtractorEngineType)
                .FirstOrDefaultAsync(cancellationToken);
        });
    }

    private string GetBucketName(FileStore fileStore)
    {
        var bucketName =
            fileStorageSettings.Value.BucketNameMappings[fileStore.Identifier];

        if (string.IsNullOrEmpty(bucketName))
        {
            throw new ArgumentException($"BucketName not found for file store {fileStore.Identifier}");
        }

        return bucketName;
    }

    public async Task<Unit> Handle(SendJobRequestCommand request, CancellationToken cancellationToken)
    {
        var file = request.File;

        var chunkingStrategy = await GetCachedApplicationChunkingStrategy(file.UploadApplicationId, cancellationToken);
        if (chunkingStrategy == null)
        {
            throw new ArgumentException($"ApplicationChunkingStrategy not found for application {file.UploadApplicationId}");
        }

        var extractionEngine = await GetCachedApplicationExtractionEngine(file.UploadApplicationId, cancellationToken);
        if (extractionEngine == null)
        {
            throw new ArgumentException($"ApplicationExtractorEngine not found for application {file.UploadApplicationId}");
        }
        
        var fileStore = await applicationFileStoreRepository.GetCachedApplicationFileStore(file.UploadApplicationId, cancellationToken);
        if (fileStore == null)
        {
            throw new ArgumentException($"ApplicationFileStore not found for application {file.UploadApplicationId}");
        }
        
        var bucketName = fileStorage.GetBucketName(fileStore.FileStore.Identifier);
        
        var jobRequest = new JobRequest
        {
            ApplicationId = file.UploadApplicationId,
            FileStoreId = fileStore.FileStoreId,
            BucketName = bucketName,
            ChunkType = chunkingStrategy.ChunkingStrategy.Identifier,
            ChunkOverlap = chunkingStrategy.Overlap ?? 0,
            ChunkSize = chunkingStrategy.ChunkSize ?? 0,
            FileId = file.Id,
            DocumentId = file.FileStoreIdentifier,
            FileExtension = file.FileExtension,
            StorageType = fileStore.FileStore.Identifier,
            ExtractorType = extractionEngine.ExtractorEngineType.Identifier
        };

        await dataManagerService.SendJobRequest(jobRequest, cancellationToken);
        
        return Unit.Value;
    }
}