using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.SearchEngineManagement.ApplicationSearchEngineHandlers
{

    public record CreateApplicationSearchEngineCommand(
        long ApplicationId,
        long SearchEngineId,
        string IndexName,
        long EmbeddingId,
        string Identifier
        ) :ICommand<CreateApplicationSearchEngineCommandResult>;
    public record CreateApplicationSearchEngineCommandResult(
        long Id,
        long ApplicationId,
        long SearchEngineId,
        string IndexName,
        long EmbeddingId,
        string Identifier
        );

    public class CreateApplicationSearchEngineCommandValidator : AbstractValidator<CreateApplicationSearchEngineCommand>
    {
        public CreateApplicationSearchEngineCommandValidator()
        {
            //TODO : Check application embeddings, embedding vector size, applicationid and search engine id
            RuleFor(x => x.IndexName).NotEmpty().MaximumLength(255);
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotEmpty().MaximumLength(50);
            
        }
    }

    internal class CreateApplicationSearchEngineCommandHandler(IRepository<ApplicationSearchEngine> repository, IMapper mapper) 
        : ICommandHandler<CreateApplicationSearchEngineCommand, CreateApplicationSearchEngineCommandResult>
    {
        private readonly IRepository<ApplicationSearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateApplicationSearchEngineCommandResult> Handle(CreateApplicationSearchEngineCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationSearchEngine>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateApplicationSearchEngineCommandResult>(entitiy);
            return result;
        }
    }
}
