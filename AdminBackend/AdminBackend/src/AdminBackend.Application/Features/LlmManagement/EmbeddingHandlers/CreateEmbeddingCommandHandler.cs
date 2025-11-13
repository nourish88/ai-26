using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.LlmManagement.EmbeddingHandlers
{

    public record CreateEmbeddingCommand(
        long LlmProviderId,
        string Url,
        string ModelName,
        int VectorSize,
        int MaxInputTokenSize) : ICommand<CreateEmbeddingCommandResult>;
    public record CreateEmbeddingCommandResult(long Id,
        long LlmProviderId,
        string Url,
        string ModelName,
        int VectorSize,
        int MaxInputTokenSize);

    public class CreateEmbeddingCommandValidator : AbstractValidator<CreateEmbeddingCommand>
    {
        public CreateEmbeddingCommandValidator()
        {
            RuleFor(x=>x.LlmProviderId).GreaterThan(0);
            RuleFor(x=>x.Url).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x=>x.ModelName).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.VectorSize).GreaterThan(0);
            RuleFor(x => x.MaxInputTokenSize).GreaterThan(0);

        }
    }

    internal class CreateEmbeddingCommandHandler(IRepository<Embedding> repository, IMapper mapper) 
        : ICommandHandler<CreateEmbeddingCommand, CreateEmbeddingCommandResult>
    {
        private readonly IRepository<Embedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateEmbeddingCommandResult> Handle(CreateEmbeddingCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Embedding>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateEmbeddingCommandResult>(entitiy);
            return result;
        }
    }
}
