using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationEmbeddingHandlers
{

    public record CreateApplicationEmbeddingCommand(
        long ApplicationId,
        long EmbeddingId) : ICommand<CreateApplicationEmbeddingCommandResult>;
    public record CreateApplicationEmbeddingCommandResult(
        long Id,
        long ApplicationId,
        long EmbeddingId);

    public class CreateApplicationEmbeddingCommandValidator : AbstractValidator<CreateApplicationEmbeddingCommand>
    {
        public CreateApplicationEmbeddingCommandValidator()
        {
            //TODO : Unique check
        }
    }

    internal class CreateApplicationEmbeddingCommandHandler(IRepository<ApplicationEmbedding> repository, IMapper mapper) 
        : ICommandHandler<CreateApplicationEmbeddingCommand, CreateApplicationEmbeddingCommandResult>
    {
        private readonly IRepository<ApplicationEmbedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateApplicationEmbeddingCommandResult> Handle(CreateApplicationEmbeddingCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationEmbedding>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateApplicationEmbeddingCommandResult>(entitiy);
            return result;
        }
    }
}
