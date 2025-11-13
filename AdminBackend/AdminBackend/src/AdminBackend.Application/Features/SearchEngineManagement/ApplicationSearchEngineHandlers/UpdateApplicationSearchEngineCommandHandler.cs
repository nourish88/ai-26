using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.SearchEngineManagement.ApplicationSearchEngineHandlers
{
    public record UpdateApplicationSearchEngineCommand(
        long Id,
        long ApplicationId,
        long SearchEngineId,
        string IndexName,
        long EmbeddingId,
        string Identifier
        ) : ICommand<UpdateApplicationSearchEngineCommandResult>;
    public record UpdateApplicationSearchEngineCommandResult(
        long Id,
        long ApplicationId,
        long SearchEngineId,
        string IndexName,
        long EmbeddingId,
        string Identifier
        );

    public class UpdateApplicationSearchEngineCommandValidator : AbstractValidator<UpdateApplicationSearchEngineCommand>
    {
        public UpdateApplicationSearchEngineCommandValidator()
        {
            //TODO : Check application embeddings, embedding vector size, applicationid and search engine id
            RuleFor(x => x.IndexName).NotEmpty().MaximumLength(255);
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotEmpty().MaximumLength(50);
        }
    }
    internal class UpdateApplicationSearchEngineCommandHandler(IRepository<ApplicationSearchEngine> repository, IMapper mapper)
        : ICommandHandler<UpdateApplicationSearchEngineCommand, UpdateApplicationSearchEngineCommandResult>
    {
        private readonly IRepository<ApplicationSearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateApplicationSearchEngineCommandResult> Handle(UpdateApplicationSearchEngineCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationSearchEngine>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateApplicationSearchEngineCommandResult>(entitiy);
            return result;
        }
    }
}
