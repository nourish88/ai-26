using AdminBackend.Application.Business;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using File = AdminBackend.Domain.Entities.File;

namespace AdminBackend.Application.Features.SearchEngineManagement.IndexManagementHandlers;

public record DeleteIndexedDocumentsCommand(long FileId) : ICommand<DeleteIndexedDocumentsCommandResult>;

public record DeleteIndexedDocumentsCommandResult(bool Success);

public class DeleteIndexedDocumentsCommandValidator : AbstractValidator<DeleteIndexedDocumentsCommand>
{
    public DeleteIndexedDocumentsCommandValidator()
    {
    }
}

internal class DeleteIndexedDocumentsCommandHandler(
    IRepository<File> fileRepository,
    ILogger<DeleteIndexCommandHandler> logger,
    IApplicationBusiness applicationBusiness
) : ICommandHandler<DeleteIndexedDocumentsCommand, DeleteIndexedDocumentsCommandResult>
{
    public async Task<DeleteIndexedDocumentsCommandResult> Handle(DeleteIndexedDocumentsCommand request,
        CancellationToken cancellationToken)
    {
        var file = await fileRepository
            .FirstOrDefaultAsync(p=> p.Id == request.FileId, cancellationToken: cancellationToken);

        if (file == null)
        {
            logger.LogError("File not found for {FileId}", request.FileId);
            
            return new DeleteIndexedDocumentsCommandResult(false);
        }

        var searchEngineMeta = await applicationBusiness.GetApplicationSearchEngine(file.UploadApplicationId, cancellationToken);
        if (searchEngineMeta == null)
        {
            return new DeleteIndexedDocumentsCommandResult(false);
        }

        var searchEngine = searchEngineMeta.SearchEngine;
        var indexName = searchEngineMeta.IndexName;

        return new DeleteIndexedDocumentsCommandResult(
            await searchEngine.DeleteDocumentsByParentIdAsync(file.FileStoreIdentifier, indexName, cancellationToken));
    }
}