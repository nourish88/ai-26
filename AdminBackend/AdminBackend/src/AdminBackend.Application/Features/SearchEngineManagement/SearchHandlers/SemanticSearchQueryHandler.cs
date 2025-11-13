using AdminBackend.Application.Business;
using AdminBackend.Application.Repositories;
using AdminBackend.Application.Services.App;
using AdminBackend.Domain.Models.Services.Search;
using AutoMapper;
using FluentValidation;
using Juga.Abstractions.Client;
using Juga.CQRS.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchHandlers
{

    public record SemanticSearchQuery(
        string query,
        int? top,
        int? skip,
        string[]? fileIdentifiers
        ) : IQuery<SemanticSearchQueryResult>;

    public record SemanticSearchQueryResponse(
        string id,
        string parentid,
        string datasourceid,
        string title,
        string soruceurl,
        string content,
        string sentiment
        );
    public record SemanticSearchQueryResult(ICollection<SemanticSearchQueryResponse> response);
    public class SemanticSearchQueryValidator : AbstractValidator<SemanticSearchQuery>
    {
        public SemanticSearchQueryValidator()
        {
            RuleFor(x => x.query).NotNull().NotEmpty();
        }
    }

    internal class SemanticSearchQueryHandler(
        ILogger<SemanticSearchQueryHandler> logger,
        IApplicationBusiness applicationBusiness,
        IMapper mapper,
        IAppService appService,
        IUserContextProvider userContextProvider,
        IApplicationRepository applicationRepository
        ) : IQueryHandler<SemanticSearchQuery, SemanticSearchQueryResult>
    {
        private readonly ILogger<SemanticSearchQueryHandler> logger = logger;
        private readonly IApplicationBusiness applicationBusiness = applicationBusiness;
        private readonly IMapper mapper = mapper;

        public async Task<SemanticSearchQueryResult> Handle(SemanticSearchQuery request, CancellationToken cancellationToken)
        {
            var appIdentifier = appService.RequesterApplicationIdentifier;
            if (appIdentifier == null)
            {
                logger.LogError($"App identifier is missing");
                return new SemanticSearchQueryResult(new List<SemanticSearchQueryResponse>());
            }
            var application = await applicationRepository.GetByIdentifierAsync( appIdentifier );
            if (application == null)
            {
                logger.LogError($"Application not found for identifier:{appIdentifier}.");
                return new SemanticSearchQueryResult(new List<SemanticSearchQueryResponse>());
            }
            if (userContextProvider.ClientId == null) {
                logger.LogError($"User id not found.");
                return new SemanticSearchQueryResult(new List<SemanticSearchQueryResponse>());
            }
            var applicationId = application.Id;
            var searchEngineMeta = await applicationBusiness.GetApplicationSearchEngine(applicationId, cancellationToken);
            if (searchEngineMeta == null)
            {
                return new SemanticSearchQueryResult(new List<SemanticSearchQueryResponse>());
            }
            var embeddingService = await applicationBusiness.GetApplicationEmbeddingService(applicationId, cancellationToken);
            if (embeddingService == null)
            {
                return new SemanticSearchQueryResult(new List<SemanticSearchQueryResponse>());
            }
            var searchEngine = searchEngineMeta.SearchEngine;
            var indexName = searchEngineMeta.IndexName;

            var input = new SemanticSearchInput(
                query:request.query,
                top:request.top,
                skip:request.skip,
                applicationId:applicationId.ToString(),
                userId: application.HasUserFile ? userContextProvider.ClientId:null,
                fileIdentifiers: application.HasUserFile ? request.fileIdentifiers:null
                );
            var output = await searchEngine.SemanticSearchAsync(embeddingService, input, indexName, cancellationToken);
            var result = mapper.Map<ICollection<SemanticSearchQueryResponse>>(output);
            return new SemanticSearchQueryResult(result);


        }
    }
}
