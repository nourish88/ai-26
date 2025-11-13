using AdminBackend.Application.Business;
using AdminBackend.Application.Services.FileStorage;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers
{

    public record DeleteFileCommand(long Id) : ICommand<DeleteFileCommandResult>;
    public record DeleteFileCommandResult(bool result);

    internal class DeleteFileCommandHandler(
        ILogger<DeleteFileCommandHandler> logger,
        IRepository<Domain.Entities.File> repository,
        IFileStorage fileStorage,
        IApplicationBusiness applicationBusiness,
        IMapper mapper
        )
        : ICommandHandler<DeleteFileCommand, DeleteFileCommandResult>
    {
        private readonly IRepository<Domain.Entities.File> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteFileCommandResult> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            var fileWithFileStoreEntity = await repository.GetFirstOrDefaultAsync(
                predicate: p => p.Id == request.Id,
                include: r => r.Include(i => i.FileStore),
                cancellationToken: cancellationToken);
            if (fileWithFileStoreEntity == null)
            {
                logger.LogWarning($"File not found with id {request.Id}");
                return new DeleteFileCommandResult(false);
            }

            var fileStoreIdentifier = fileWithFileStoreEntity.FileStoreIdentifier;
            var applicationId = fileWithFileStoreEntity.UploadApplicationId;

            // Delete from storage
            if (fileWithFileStoreEntity.FileStore != null)
            {
                var storageIdentifier = fileWithFileStoreEntity.FileStore.Identifier;
                await fileStorage.DeleteObjectRecursivelyAsync(storageIdentifier, fileStoreIdentifier, cancellationToken);
            }

            //Delete from application vector store
            var searchEngine = await applicationBusiness.GetApplicationSearchEngine(applicationId, cancellationToken);
            if (searchEngine != null)
            {
                await searchEngine.SearchEngine.DeleteDocumentsByParentIdAsync(fileStoreIdentifier, searchEngine.IndexName, cancellationToken);
            }

            //Delete from db
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteFileCommandResult(true);
        }
    }
}
