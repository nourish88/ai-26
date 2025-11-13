using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ChunkingManagement.ApplicationChunkingStrategyHandlers
{
    public record UpdateApplicationChunkingStrategyCommand(
        long Id,
        long ApplicationId,
        long ChunkingStrategyId,
        int? ChunkSize,
        int? Overlap,
        string? Seperator) : ICommand<UpdateApplicationChunkingStrategyCommandResult>;
    public record UpdateApplicationChunkingStrategyCommandResult(
        long Id,
        long ApplicationId,
        long ChunkingStrategyId,
        int? ChunkSize,
        int? Overlap,
        string? Seperator) ;

    public class UpdateApplicationChunkingStrategyCommandValidator : AbstractValidator<UpdateApplicationChunkingStrategyCommand>
    {
        public UpdateApplicationChunkingStrategyCommandValidator()
        {
            // TOTO: Get Chunking strategy and check chunksize and overlap requirement
            RuleFor(x => x.ApplicationId).GreaterThan(0);
            RuleFor(x => x.ChunkingStrategyId).GreaterThan(0);
        }
    }
    internal class UpdateApplicationChunkingStrategyCommandHandler(IRepository<ApplicationChunkingStrategy> repository, IMapper mapper)
        : ICommandHandler<UpdateApplicationChunkingStrategyCommand, UpdateApplicationChunkingStrategyCommandResult>
    {
        private readonly IRepository<ApplicationChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateApplicationChunkingStrategyCommandResult> Handle(UpdateApplicationChunkingStrategyCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationChunkingStrategy>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateApplicationChunkingStrategyCommandResult>(entitiy);
            return result;
        }
    }
}
