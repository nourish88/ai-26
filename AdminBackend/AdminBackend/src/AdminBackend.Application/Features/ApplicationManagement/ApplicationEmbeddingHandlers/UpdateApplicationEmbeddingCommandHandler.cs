using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationEmbeddingHandlers
{
    public record UpdateApplicationEmbeddingCommand(
        long Id,
        long ApplicationId,
        long EmbeddingId) : ICommand<UpdateApplicationEmbeddingCommandResult>;
    public record UpdateApplicationEmbeddingCommandResult(
        long Id,
        long ApplicationId,
        long EmbeddingId) ;

    public class UpdateApplicationEmbeddingCommandValidator : AbstractValidator<UpdateApplicationEmbeddingCommand>
    {
        public UpdateApplicationEmbeddingCommandValidator()
        {
            //TODO : Unique check
        }
    }
    internal class UpdateApplicationEmbeddingCommandHandler(IRepository<ApplicationEmbedding> repository, IMapper mapper)
        : ICommandHandler<UpdateApplicationEmbeddingCommand, UpdateApplicationEmbeddingCommandResult>
    {
        private readonly IRepository<ApplicationEmbedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateApplicationEmbeddingCommandResult> Handle(UpdateApplicationEmbeddingCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationEmbedding>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateApplicationEmbeddingCommandResult>(entitiy);
            return result;
        }
    }
}
