using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ChunkingManagement.ApplicationChunkingStrategyHandlers
{

    public record DeleteApplicationChunkingStrategyCommand(long Id):ICommand<DeleteApplicationChunkingStrategyCommandResult>;
    public record DeleteApplicationChunkingStrategyCommandResult(bool result);    

    internal class DeleteApplicationChunkingStrategyCommandHandler(IRepository<ApplicationChunkingStrategy> repository, IMapper mapper) 
        : ICommandHandler<DeleteApplicationChunkingStrategyCommand, DeleteApplicationChunkingStrategyCommandResult>
    {
        private readonly IRepository<ApplicationChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteApplicationChunkingStrategyCommandResult> Handle(DeleteApplicationChunkingStrategyCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteApplicationChunkingStrategyCommandResult(true);
        }
    }
}
