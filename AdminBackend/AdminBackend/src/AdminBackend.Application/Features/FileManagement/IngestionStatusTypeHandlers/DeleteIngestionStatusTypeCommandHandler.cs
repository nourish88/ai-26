using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.IngestionStatusTypeHandlers
{

    public record DeleteIngestionStatusTypeCommand(long Id):ICommand<DeleteIngestionStatusTypeCommandResult>;
    public record DeleteIngestionStatusTypeCommandResult(bool result);    

    internal class DeleteIngestionStatusTypeCommandHandler(IRepository<IngestionStatusType> repository, IMapper mapper) 
        : ICommandHandler<DeleteIngestionStatusTypeCommand, DeleteIngestionStatusTypeCommandResult>
    {
        private readonly IRepository<IngestionStatusType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteIngestionStatusTypeCommandResult> Handle(DeleteIngestionStatusTypeCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteIngestionStatusTypeCommandResult(true);
        }
    }
}
