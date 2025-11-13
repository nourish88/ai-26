using AdminBackend.Application.Business;
using AdminBackend.Application.Services.Search;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Microsoft.Extensions.Logging;

namespace AdminBackend.Application.Features.SearchEngineManagement.IndexManagementHandlers
{
    public record CreateIndexCommand(long ApplicationId) : ICommand<CreateIndexCommandResult>;
    public record CreateIndexCommandResult(bool IsCreated);

    public class CreateIndexCommandValidator : AbstractValidator<CreateIndexCommand>
    {
        public CreateIndexCommandValidator()
        {
        }
    }

    internal class CreateIndexCommandHandler(
        ILogger<CreateIndexCommandHandler> logger
        , IApplicationBusiness applicationBusiness
        , IIndexDefinitionFactory indexDefinitionFactory        
        )
        : ICommandHandler<CreateIndexCommand, CreateIndexCommandResult>
    {
        private readonly ILogger<CreateIndexCommandHandler> logger = logger;
        private readonly IApplicationBusiness applicationBusiness = applicationBusiness;
        private readonly IIndexDefinitionFactory indexDefinitionFactory = indexDefinitionFactory;

        public async Task<CreateIndexCommandResult> Handle(CreateIndexCommand request, CancellationToken cancellationToken)
        {

            var searchEngineMeta = await applicationBusiness.GetApplicationSearchEngine(request.ApplicationId, cancellationToken);
            if (searchEngineMeta == null)
            {
                return await Task.FromResult(new CreateIndexCommandResult(false));
            }
            
            var searchEngine = searchEngineMeta.SearchEngine;
            var indexName = searchEngineMeta.IndexName;
            var searchEngineType = searchEngineMeta.SearchEngineType;
            var indexIdentifier = searchEngineMeta.IndexIdentifier;
            

            var indexDefinition = indexDefinitionFactory.GetIndexDefinition(searchEngineType, indexIdentifier);
            if (indexDefinition == null)
            {
                logger.LogError($"Index definition not found for {searchEngineType}, {indexIdentifier}");
                return await Task.FromResult(new CreateIndexCommandResult(false));
            }
            
            var createIndexResult = await searchEngine.CreateIndexAsync(indexDefinition, indexName, cancellationToken);
            return await Task.FromResult(new CreateIndexCommandResult(createIndexResult));

        }
    }
}
