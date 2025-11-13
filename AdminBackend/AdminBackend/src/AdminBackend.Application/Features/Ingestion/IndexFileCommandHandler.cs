using System.Text;
using AdminBackend.Application.Business;
using AdminBackend.Application.Features.FileManagement.FileHandlers;
using AdminBackend.Application.Repositories;
using AdminBackend.Application.Services.AI;
using AdminBackend.Application.Services.FileStorage;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Models.Services.Search;
using Juga.CQRS.Abstractions;
using MassTransit.SagaStateMachine;
using MediatR;
using Microsoft.Extensions.Logging;
using File = AdminBackend.Domain.Entities.File;

namespace AdminBackend.Application.Features.Ingestion;

public record IndexFileCommand(File File) : ICommand<IndexFileCommandResult>;

public record IndexFileCommandResult(bool Success);

internal class IndexFileCommandHandler(
    ILogger<IndexFileCommandHandler> logger,
    IApplicationFileStoreRepository applicationFileStoreRepository,
    IApplicationBusiness applicationBusiness,
    ISender sender,
    IFileStorage fileStorage) : ICommandHandler<IndexFileCommand, IndexFileCommandResult>
{
    private async Task LoadEmbeddings(IEmbeddingService embeddingService,
        IReadOnlyList<ElasticSearchEntity> entities, CancellationToken cancellationToken)
    {

        try
        {
            foreach (var entity in entities)
            {
                var embedding = await embeddingService.GetEmbeddingAsync(entity.content, cancellationToken);
                if (embedding == null)
                {
                    throw new InvalidOperationException($"Embedding is not created for chunk {entity.id} for file {entity.parentid}");
                }
                entity.contentvector = embedding.Value.ToArray().ToList();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Load embeddings failed.");
            throw;
        }

        //var tasks = entities.Select(async p =>
        //{
        //    var embedding = await embeddingService.GetEmbeddingAsync(p.content, cancellationToken);
        //    if (embedding == null)
        //    {
        //        throw new InvalidOperationException($"Embedding is not created for chunk {p.id} for file {p.parentid}");
        //    }

        //    p.contentvector = embedding.Value.ToArray().ToList();
        //    return Task.CompletedTask;
        //});

        //await Task.WhenAll(tasks);
    }

    private async Task LoadContents(IReadOnlyList<ElasticSearchEntity> entities, CancellationToken cancellationToken)
    {
        var tasks = entities.Select(async p =>
        {
            var data = await fileStorage.GetObjectAsync(
                p.storeidentifier, p.filepath, cancellationToken);

            if (data.Length == 0)
            {
                throw new InvalidOperationException($"Content is not found for chunk {p.id} for file {p.parentid}");
            }

            p.content = Encoding.UTF8.GetString(data);
            return Task.CompletedTask;
        });

        await Task.WhenAll(tasks);
    }

    private async Task<IReadOnlyList<ElasticSearchEntity>> CreateIndexDocument(IEmbeddingService embeddingService,
        File file,
        CancellationToken cancellationToken)
    {
        var fileStore =
            await applicationFileStoreRepository.GetCachedApplicationFileStore(file.UploadApplicationId,
                cancellationToken);
        if (fileStore == null)
        {
            throw new ArgumentException($"ApplicationFileStore not found for application {file.UploadApplicationId}");
        }

        var bucketName = fileStorage.GetBucketName(fileStore.FileStore.Identifier);

        var chunkPaths = await fileStorage.ListObjectsAsync(
            fileStore.FileStore.Identifier, $"{file.FileStoreIdentifier}/chunks/", cancellationToken);

        if (chunkPaths.Count == 0)
        {
            logger.LogWarning("No chunks found for file {fileId}", file.Id);
            return [];
        }

        var entities = chunkPaths.Select(p => new ElasticSearchEntity
        {
            id = Path.GetFileNameWithoutExtension(p),
            parentid = file.FileStoreIdentifier,
            applicationid = file.UploadApplicationId.ToString(),
            title = file.Title ?? "",
            filepath = p,
            content = "",
            keywords = [],
            sentiment = "",
            userid = file.CreatedBy ?? "",
            sourceurl = "",
            datasourceid = "",
            storeidentifier = fileStore.FileStore.Identifier,
            bucket = bucketName,
            filetype = file.FileType == null ? 0 : (int)file.FileType.Id,
            contentvector = []
        }).ToList();

        await LoadContents(entities, cancellationToken);
        await LoadEmbeddings(embeddingService, entities, cancellationToken);

        return entities;
    }

    public async Task<IndexFileCommandResult> Handle(IndexFileCommand request, CancellationToken cancellationToken)
    {
        var searchEngineMeta = await applicationBusiness.GetApplicationSearchEngine(request.File.UploadApplicationId, cancellationToken);
        if (searchEngineMeta == null)
        {
            return new IndexFileCommandResult(false);
        }
        var embeddingService = await applicationBusiness.GetApplicationEmbeddingService(request.File.UploadApplicationId, cancellationToken);
        if (embeddingService == null)
        {
            return new IndexFileCommandResult(false);
        }
        var searchEngine = searchEngineMeta.SearchEngine;
        var indexName = searchEngineMeta.IndexName;
        bool result = false;
        string? errorDetail = null;

        try
        {
            var indexedDocuments = await CreateIndexDocument(embeddingService, request.File, cancellationToken);

            if (indexedDocuments.Count != 0)
            {
                result = await searchEngine.BulkInsertAsync(indexedDocuments, indexName, cancellationToken);
                if (!result)
                {
                    errorDetail = "Unable to send bulk indexing request";
                }
            }
            else
            {
                errorDetail = "Couldn't find any chunks";
            }

            return new IndexFileCommandResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while indexing file {fileId}", request.File.Id);
            errorDetail = ex.Message;
            return new IndexFileCommandResult(false);
        }
        finally
        {
            if (!result)
            {
                // Do not pass the cancellation token here. Let the flow continue
                await sender.Send(new UpdateFileErrorStatusCommand(request.File.Id, errorDetail!));
            }
            else
            {
                // Do not pass the cancellation token here. Let the flow continue
                await sender.Send(new UpdateFileStatusCommand(request.File.Id, IngestionStatusTypes.Processed));
            }
        }
    }
}