using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ChunkingManagement.ChunkingStrategyHandlers
{

    public record DeleteChunkingStrategyCommand(long Id):ICommand<DeleteChunkingStrategyCommandResult>;
    public record DeleteChunkingStrategyCommandResult(bool result);    

    internal class DeleteChunkingStrategyCommandHandler(IRepository<ChunkingStrategy> repository, IMapper mapper) 
        : ICommandHandler<DeleteChunkingStrategyCommand, DeleteChunkingStrategyCommandResult>
    {
        private readonly IRepository<ChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteChunkingStrategyCommandResult> Handle(DeleteChunkingStrategyCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteChunkingStrategyCommandResult(true);
        }
    }
}
