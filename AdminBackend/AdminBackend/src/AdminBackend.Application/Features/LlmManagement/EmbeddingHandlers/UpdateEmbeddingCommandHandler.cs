using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.LlmManagement.EmbeddingHandlers
{
    public record UpdateEmbeddingCommand(long Id,
        long LlmProviderId,
        string Url,
        string ModelName,
        int VectorSize,
        int MaxInputTokenSize) : ICommand<UpdateEmbeddingCommandResult>;
    public record UpdateEmbeddingCommandResult(long Id,
        long LlmProviderId,
        string Url,
        string ModelName,
        int VectorSize,
        int MaxInputTokenSize) ;

    public class UpdateEmbeddingCommandValidator : AbstractValidator<UpdateEmbeddingCommand>
    {
        public UpdateEmbeddingCommandValidator()
        {
            RuleFor(x => x.LlmProviderId).GreaterThan(0);
            RuleFor(x => x.Url).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x => x.ModelName).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.VectorSize).GreaterThan(0);
            RuleFor(x => x.MaxInputTokenSize).GreaterThan(0);
        }
    }
    internal class UpdateEmbeddingCommandHandler(IRepository<Embedding> repository, IMapper mapper)
        : ICommandHandler<UpdateEmbeddingCommand, UpdateEmbeddingCommandResult>
    {
        private readonly IRepository<Embedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateEmbeddingCommandResult> Handle(UpdateEmbeddingCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Embedding>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateEmbeddingCommandResult>(entitiy);
            return result;
        }
    }
}
