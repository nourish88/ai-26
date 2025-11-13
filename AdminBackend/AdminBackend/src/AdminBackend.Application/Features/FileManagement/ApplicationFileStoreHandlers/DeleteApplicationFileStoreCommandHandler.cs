using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.ApplicationFileStoreHandlers
{

    public record DeleteApplicationFileStoreCommand(long Id):ICommand<DeleteApplicationFileStoreCommandResult>;
    public record DeleteApplicationFileStoreCommandResult(bool result);    

    internal class DeleteApplicationFileStoreCommandHandler(IRepository<ApplicationFileStore> repository, IMapper mapper) 
        : ICommandHandler<DeleteApplicationFileStoreCommand, DeleteApplicationFileStoreCommandResult>
    {
        private readonly IRepository<ApplicationFileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteApplicationFileStoreCommandResult> Handle(DeleteApplicationFileStoreCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteApplicationFileStoreCommandResult(true);
        }
    }
}
