using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ChunkingManagement.ChunkingStrategyHandlers
{

    public record CreateChunkingStrategyCommand(
        string Identifier,
        bool IsChunkingSizeRequired,
        bool IsOverlapRequired) : ICommand<CreateChunkingStrategyCommandResult>;
    public record CreateChunkingStrategyCommandResult(
        long Id,
        string Identifier,
        bool IsChunkingSizeRequired,
        bool IsOverlapRequired);

    public class CreateChunkingStrategyCommandValidator : AbstractValidator<CreateChunkingStrategyCommand>
    {
        public CreateChunkingStrategyCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotNull().NotEmpty().MaximumLength(50);
        }
    }

    internal class CreateChunkingStrategyCommandHandler(IRepository<ChunkingStrategy> repository, IMapper mapper) 
        : ICommandHandler<CreateChunkingStrategyCommand, CreateChunkingStrategyCommandResult>
    {
        private readonly IRepository<ChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateChunkingStrategyCommandResult> Handle(CreateChunkingStrategyCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ChunkingStrategy>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateChunkingStrategyCommandResult>(entitiy);
            return result;
        }
    }
}
