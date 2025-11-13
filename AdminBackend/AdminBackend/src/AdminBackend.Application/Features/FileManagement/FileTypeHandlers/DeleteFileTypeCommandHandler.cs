using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.FileTypeHandlers
{

    public record DeleteFileTypeCommand(long Id):ICommand<DeleteFileTypeCommandResult>;
    public record DeleteFileTypeCommandResult(bool result);    

    internal class DeleteFileTypeCommandHandler(IRepository<FileType> repository, IMapper mapper) 
        : ICommandHandler<DeleteFileTypeCommand, DeleteFileTypeCommandResult>
    {
        private readonly IRepository<FileType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteFileTypeCommandResult> Handle(DeleteFileTypeCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteFileTypeCommandResult(true);
        }
    }
}
