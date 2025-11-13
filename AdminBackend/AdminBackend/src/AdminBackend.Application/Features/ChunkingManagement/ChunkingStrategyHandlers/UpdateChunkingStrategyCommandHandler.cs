using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ChunkingManagement.ChunkingStrategyHandlers
{
    public record UpdateChunkingStrategyCommand(
        long Id,
        string Identifier,
        bool IsChunkingSizeRequired,
        bool IsOverlapRequired) : ICommand<UpdateChunkingStrategyCommandResult>;
    public record UpdateChunkingStrategyCommandResult(
        long Id,
        string Identifier,
        bool IsChunkingSizeRequired,
        bool IsOverlapRequired) ;

    public class UpdateChunkingStrategyCommandValidator : AbstractValidator<UpdateChunkingStrategyCommand>
    {
        public UpdateChunkingStrategyCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotNull().NotEmpty().MaximumLength(50);
        }
    }
    internal class UpdateChunkingStrategyCommandHandler(IRepository<ChunkingStrategy> repository, IMapper mapper)
        : ICommandHandler<UpdateChunkingStrategyCommand, UpdateChunkingStrategyCommandResult>
    {
        private readonly IRepository<ChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateChunkingStrategyCommandResult> Handle(UpdateChunkingStrategyCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ChunkingStrategy>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateChunkingStrategyCommandResult>(entitiy);
            return result;
        }
    }
}
