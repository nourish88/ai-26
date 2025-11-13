using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ChunkingManagement.ApplicationChunkingStrategyHandlers
{

    public record CreateApplicationChunkingStrategyCommand(
        long ApplicationId,
        long ChunkingStrategyId,
        int? ChunkSize,
        int? Overlap,
        string? Seperator) : ICommand<CreateApplicationChunkingStrategyCommandResult>;
    public record CreateApplicationChunkingStrategyCommandResult(
        long Id,
        long ApplicationId,
        long ChunkingStrategyId,
        int? ChunkSize,
        int? Overlap,
        string? Seperator);

    public class CreateApplicationChunkingStrategyCommandValidator : AbstractValidator<CreateApplicationChunkingStrategyCommand>
    {
        public CreateApplicationChunkingStrategyCommandValidator()
        {
            // TOTO: Get Chunking strategy and check chunksize and overlap requirement
            RuleFor(x => x.ApplicationId).GreaterThan(0);
            RuleFor(x => x.ChunkingStrategyId).GreaterThan(0);

        }
    }

    internal class CreateApplicationChunkingStrategyCommandHandler(IRepository<ApplicationChunkingStrategy> repository, IMapper mapper) 
        : ICommandHandler<CreateApplicationChunkingStrategyCommand, CreateApplicationChunkingStrategyCommandResult>
    {
        private readonly IRepository<ApplicationChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateApplicationChunkingStrategyCommandResult> Handle(CreateApplicationChunkingStrategyCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationChunkingStrategy>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateApplicationChunkingStrategyCommandResult>(entitiy);
            return result;
        }
    }
}
