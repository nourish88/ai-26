using AdminBackend.Application.Features.Ingestion;
using AdminBackend.Application.Services.AI;
using AdminBackend.Application.Services.Search;
using AdminBackend.Application.Settings;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AdminBackend.Domain.Helpers;

namespace AdminBackend.Application.Business
{
    public record ApplicationSearchEngineMeta(ISearchEngine SearchEngine, SearchEngineTypes SearchEngineType, string IndexIdentifier, string IndexName);
    public class ApplicationBusiness : IApplicationBusiness
    {
        private readonly ILogger<ApplicationBusiness> logger;
        private readonly IRepository<ApplicationSearchEngine> applicationSearchEnginesRepo;
        private readonly IRepository<ApplicationEmbedding> applicationEmbeddingRepository;
        private readonly IRepository<Domain.Entities.File> fileRepository;
        private readonly ISearchEngineFactory searchEngineFactory;
        private readonly IEmbeddingServiceFactory embeddingServiceFactory;
        private readonly IndexSettings indexSettings;
        private readonly EmbeddingSettings embeddingSettings;

        public ApplicationBusiness(
            ILogger<ApplicationBusiness> logger
            , IRepository<ApplicationSearchEngine> applicationSearchEnginesRepo
            , IRepository<ApplicationEmbedding> applicationEmbeddingRepository
            , IRepository<Domain.Entities.File> fileRepository
            , IOptions<IndexSettings> indexSettings
            , IOptions<EmbeddingSettings> embeddingSettings
            , ISearchEngineFactory searchEngineFactory
            , IEmbeddingServiceFactory embeddingServiceFactory
            )
        {
            this.logger = logger;
            this.applicationSearchEnginesRepo = applicationSearchEnginesRepo;
            this.applicationEmbeddingRepository = applicationEmbeddingRepository;
            this.fileRepository = fileRepository;
            this.searchEngineFactory = searchEngineFactory;
            this.embeddingServiceFactory = embeddingServiceFactory;
            this.indexSettings = indexSettings.Value;
            this.embeddingSettings = embeddingSettings.Value;
        }
        public async Task<IEmbeddingService?> GetApplicationEmbeddingService(long applicationId, CancellationToken cancellationToken = default)
        {
            var applicationEmbedding = await applicationEmbeddingRepository.GetFirstOrDefaultAsync(
            predicate: p => p.ApplicationId == applicationId,
            include: r => r.Include(i => i.Embedding),
            cancellationToken: cancellationToken);

            if (applicationEmbedding == null)
            {
                logger.LogError($"Application embedding engine is missing for {applicationId}");
                return null;
            }

            var llmProviderType = (LlmProviderTypes)applicationEmbedding.Embedding.LlmProviderId;
            var embeddingUrl = applicationEmbedding.Embedding.Url;
            var embeddingModelName = applicationEmbedding.Embedding.ModelName;

            var apiKey = embeddingSettings.ServiceKeyMappings.GetValueOrNull(embeddingModelName);
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = embeddingSettings.ServiceKeyMappings.GetValueOrNull(ConfigurationConstants.EmbeddingServiceDefaultApiKeyDictionaryKey);
            }

            var embeddingConfiguration = new Dictionary<string, string?>
        {
            { ConfigurationConstants.EmbeddingServiceUriDictionaryKey, embeddingUrl },
            {
                ConfigurationConstants.EmbeddingServiceApiKeyDictionaryKey, apiKey
            },
            { ConfigurationConstants.EmbeddingServiceModelNameDictionaryKey, embeddingModelName }
        };

            var embeddingService = embeddingServiceFactory.GetEmbeddingService(llmProviderType);
            if (embeddingService == null)
            {
                logger.LogError($"Embedding engine not found for {llmProviderType}");
                return null;
            }
            embeddingService.Configure(embeddingConfiguration);
            return embeddingService;
        }

        public async Task<ApplicationSearchEngineMeta?> GetApplicationSearchEngine(long applicationId, CancellationToken cancellationToken = default)
        {
            var applicationSearchEngine = await applicationSearchEnginesRepo.GetFirstOrDefaultAsync(
            predicate: p => p.ApplicationId == applicationId,
            include: r => r.Include(i => i.SearchEngine),
            cancellationToken: cancellationToken);

            if (applicationSearchEngine == null)
            {
                logger.LogError($"Application search engine is missing for {applicationId}");

                return null;
            }

            var searchEngineType = (SearchEngineTypes)applicationSearchEngine.SearchEngine.SearchEngineTypeId;
            var indexName = applicationSearchEngine.IndexName;
            var indexIdentifier = applicationSearchEngine.Identifier;
            var searchEngineUrl = applicationSearchEngine.SearchEngine.Url;
            var searchEngineIdentifier = applicationSearchEngine.SearchEngine.Identifier;
            var searchEngineConfiguration = new Dictionary<string, string?>()
            {
                { ConfigurationConstants.SearchServiceUriDictionaryKey, searchEngineUrl },
                {
                    ConfigurationConstants.SearchServiceUserNameDictionaryKey,
                    indexSettings.ServiceUserNameMappings.GetValueOrNull(searchEngineIdentifier)
                },
                {
                    ConfigurationConstants.SearchServicePasswordDictionaryKey,
                    indexSettings.ServicePasswordMappings.GetValueOrNull(searchEngineIdentifier)
                }
            };

            var searchEngine = searchEngineFactory.GetEngine(searchEngineType, indexIdentifier);
            if (searchEngine == null)
            {
                logger.LogError($"Search engine not found for {searchEngineType}, {indexIdentifier}");
                return null;
            }
            searchEngine.Configure(searchEngineConfiguration);
            return new ApplicationSearchEngineMeta(searchEngine, searchEngineType, indexIdentifier, indexName);
        }

        public async Task<bool> IsUserFileOwner(string userId, long fileId, CancellationToken cancellationToken = default)
        {
            var file = await fileRepository.GetFirstOrDefaultAsync(predicate: p => p.Id == fileId, cancellationToken: cancellationToken);
            return file != null && file.CreatedBy == userId;
        }

        public async Task<bool> IsUserFileOwnerInApplication(string userId, long fileId, long applicationId, CancellationToken cancellationToken = default)
        {
            var file = await fileRepository.GetFirstOrDefaultAsync(predicate: p => p.Id == fileId && p.UploadApplicationId == applicationId, cancellationToken: cancellationToken);
            return file != null && file.CreatedBy == userId;
        }
    }
}
