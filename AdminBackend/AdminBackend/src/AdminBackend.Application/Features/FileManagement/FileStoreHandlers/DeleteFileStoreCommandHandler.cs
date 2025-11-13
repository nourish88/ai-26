using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.FileStoreHandlers
{

    public record DeleteFileStoreCommand(long Id):ICommand<DeleteFileStoreCommandResult>;
    public record DeleteFileStoreCommandResult(bool result);    

    internal class DeleteFileStoreCommandHandler(IRepository<FileStore> repository, IMapper mapper) 
        : ICommandHandler<DeleteFileStoreCommand, DeleteFileStoreCommandResult>
    {
        private readonly IRepository<FileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteFileStoreCommandResult> Handle(DeleteFileStoreCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteFileStoreCommandResult(true);
        }
    }
}
