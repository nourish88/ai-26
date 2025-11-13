using AdminBackend.Application.Services.AI;
using AdminBackend.Application.Services.Search;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using AdminBackend.Domain.Models.Services.Search;
using Ardalis.GuardClauses;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Transport;
using Microsoft.Extensions.Logging;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elastic.Clients.Elasticsearch.Serialization;
using Elastic.Clients.Elasticsearch.Core.Search;

namespace AdminBackend.Infrastructure.Services.Search.SearchEngines
{
    public class ElasticSearchEngine : ISearchEngine
    {
        protected ILogger<ElasticSearchEngine> logger;
        private string serviceUri = "";
        private string userName = "";
        private string password = "";
        private bool isConfigured = false;
        private ElasticsearchClient elasticsearchClient;

        public ElasticSearchEngine(ILogger<ElasticSearchEngine> logger)
        {
            this.logger = logger;
        }

        public SearchEngineTypes SearchEngineType => SearchEngineTypes.Elastic;

        public string[] SupportedIndexIdentifiers => [];

        public void Configure(IDictionary<string, string?> configuration)
        {
            if (!configuration.TryGetValue(ConfigurationConstants.SearchServiceUriDictionaryKey, out serviceUri))
            {
                throw new ArgumentException("Configuration key missing.",
                    ConfigurationConstants.SearchServiceUriDictionaryKey);
            }

            if (!configuration.TryGetValue(ConfigurationConstants.SearchServiceUserNameDictionaryKey, out userName))
            {
                throw new ArgumentException("Configuration key missing.",
                    ConfigurationConstants.SearchServiceUserNameDictionaryKey);
            }

            if (!configuration.TryGetValue(ConfigurationConstants.SearchServicePasswordDictionaryKey, out password))
            {
                throw new ArgumentException("Configuration key missing.",
                    ConfigurationConstants.SearchServicePasswordDictionaryKey);
            }

            isConfigured = true;
        }

        public async Task<bool> DeleteDocumentsByParentIdAsync(string parentId, string indexName,
            CancellationToken cancellationToken = default)
        {
            EnsureConfigured();
            Guard.Against.NullOrWhiteSpace(indexName, nameof(indexName));

            try
            {
                var response = await ElasticsearchClient
                    .DeleteByQueryAsync<ElasticSearchEntity>(b =>
                            b
                                .Query(q => q.Term(t =>
                                    t.Field(field => field.parentid).Value(parentId)))
                                .Indices(indexName), cancellationToken
                    );

                if (response.IsValidResponse)
                {
                    return true;
                }

                logger.LogError("Unable to delete documents for index: {IndexName}, parentId: {ParentId}: {@Error}",
                    indexName, parentId,
                    response.ElasticsearchServerError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Delete documents for index: {IndexName}, parentId: {ParentId} failed", indexName, parentId);
            }

            return false;
        }

        public async Task<bool> BulkInsertAsync(IReadOnlyList<ElasticSearchEntity> entities, string indexName,
            CancellationToken cancellationToken = default)
        {
            EnsureConfigured();
            Guard.Against.NullOrWhiteSpace(indexName, nameof(indexName));
            Guard.Against.Null(entities);

            try
            {
                var observableBulk = ElasticsearchClient.BulkAll(entities
                    , cfg => cfg
                    .MaxDegreeOfParallelism(8)
                    .BackOffTime(TimeSpan.FromSeconds(100))
                    .BackOffRetries(3)
                    .Size(100)
                    .RefreshOnCompleted()
                    .Index(indexName)
                    .BufferToBulk((r, buffer) => r.IndexMany(buffer))
                    , cancellationToken);

                var waitHandle = new ManualResetEvent(false);
                Exception ex = null;

                // what to do on each call, when an exception is thrown, and 
                // when the bulk all completes
                var bulkAllObserver = new BulkAllObserver(
                    onNext: bulkAllResponse =>
                    {
                        // do something after each bulk request
                    },
                    onError: exception =>
                    {
                        // do something with exception thrown
                        ex = exception;
                        waitHandle.Set();
                    },
                    onCompleted: () =>
                    {
                        // do something when all bulk operations complete
                        waitHandle.Set();
                    });

                observableBulk.Subscribe(bulkAllObserver);

                // wait for handle to be set.
                waitHandle.WaitOne();

                if (ex != null)
                {
                    throw ex;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Bulk indexing for {IndexName} failed", indexName);
            }

            return false;
        }

        public async Task<bool> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default)
        {
            EnsureConfigured();
            Guard.Against.NullOrWhiteSpace(indexName, nameof(indexName));

            try
            {
                var response =
                    await ElasticsearchClient.Indices.DeleteAsync(new DeleteIndexRequest(indexName), cancellationToken);
                if (response.IsValidResponse)
                {
                    return true;
                }

                logger.LogError("{IndexName} index deletion failed: {@ElasticsearchServerError}", indexName,
                    response.ElasticsearchServerError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{IndexName} index deletion failed.", indexName);
            }

            return false;
        }

        public async Task<bool> CreateIndexAsync(IIndexDefinition indexDefinition, string indexName,
            CancellationToken cancellationToken = default)
        {
            EnsureConfigured();
            Guard.Against.Null(indexDefinition, nameof(indexDefinition));
            Guard.Against.NullOrWhiteSpace(indexName, nameof(indexName));
            Guard.Against.Expression((val) => { return val != SearchEngineTypes.Elastic; },
                indexDefinition.SearchEngineType,
                message: $"Invalid SearchEngineType {indexDefinition.SearchEngineType}",
                parameterName: nameof(indexDefinition.SearchEngineType)
            );

            var definition = indexDefinition.Definition(indexName);
            if (definition == null)
            {
                logger.LogError($"{indexName} index creation failed. Reason : Cannot find index definition.");
                return false;
            }
            var createIndexRequest = definition as CreateIndexRequest;
            if (createIndexRequest == null)
            {
                logger.LogError($"{indexName} index creation failed. Reason : Cannot create 'CreateIndexRequest'.");
                return false;
            }

            try
            {
                var indexCreationResult =
                    await ElasticsearchClient.Indices.CreateAsync(createIndexRequest,
                        cancellationToken);
                if (indexCreationResult.IsValidResponse)
                {
                    logger.LogInformation($"{indexName} index created.");
                    return true;
                }
                else
                {
                    Exception? originalException;
                    if (indexCreationResult.TryGetOriginalException(out originalException) && originalException != null)
                    {
                        logger.LogError(originalException, $"{indexName} index creation failed.");
                    }
                    else
                    {
                        logger.LogError(
                            $"{indexName} index creation failed. Reason : {indexCreationResult.ElasticsearchServerError}");
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{indexName} index creation failed.");
                return false;
            }
        }

        public async Task<IEnumerable<SemanticSearchOutput>?> SemanticSearchAsync(
            IEmbeddingService embeddingService,
            SemanticSearchInput semanticSearchInput,
            string indexName,
            CancellationToken cancellationToken = default)
        {
            EnsureConfigured();
            Guard.Against.Null(semanticSearchInput, nameof(semanticSearchInput));
            Guard.Against.Null(embeddingService, nameof(embeddingService));
            Guard.Against.NullOrWhiteSpace(semanticSearchInput.query, nameof(semanticSearchInput.query));
            Guard.Against.NullOrWhiteSpace(indexName, nameof(indexName));

            var embeddings = await embeddingService.GetEmbeddingAsync(semanticSearchInput.query, cancellationToken);
            if (embeddings == null)
            {
                logger.LogError($"Embedding creation failed for {semanticSearchInput.query}");
                return null;
            }
            /*where((applicationid = semanticSearchInput.applicationId and filetype = FileType.Application) OR(applicationid = semanticSearchInput.applicationId and filetype = FileType.Personal and userid = semanticSearchInput.userId)) 
             * and(parentid in semanticSearchInput.fileIdentifiers)
            */

            var knnFilters = new List<Query>();

            var applicationBool = new BoolQuery
            {
                Must = new List<Query>
                        {
                            new TermQuery
                            {
                                Field = Field.FromString(nameof(ElasticSearchEntity.applicationid)),
                                Value = semanticSearchInput.applicationId
                            },
                            new TermQuery
                            {
                                Field = Field.FromString(nameof(ElasticSearchEntity.filetype)),
                                Value = (int)FileTypes.Application
                            }
                        }
            };

            var userBool = new BoolQuery
            {
                Must = new List<Query>
                        {
                            new TermQuery
                            {
                                Field = Field.FromString(nameof(ElasticSearchEntity.applicationid)),
                                Value = semanticSearchInput.applicationId
                            },
                            new TermQuery
                            {
                                Field = Field.FromString(nameof(ElasticSearchEntity.filetype)),
                                Value = (int)FileTypes.Personal
                            },
                            new TermQuery
                            {
                                Field = Field.FromString(nameof(ElasticSearchEntity.userid)),
                                Value = semanticSearchInput.userId
                            }
                        }
            };


            if (semanticSearchInput.fileIdentifiers != null && semanticSearchInput.fileIdentifiers.Any())
            {
                var fileFilter = new TermsQuery
                {
                    Field = Field.FromString(nameof(ElasticSearchEntity.parentid)),
                    Terms = new TermsQueryField(CreateTermsQuery(semanticSearchInput.fileIdentifiers).ToList())
                };
                applicationBool.Must.Add(fileFilter);
                userBool.Must.Add(fileFilter);
            }         

            var orBool = new BoolQuery
            {
                //Where application type and application id
                Should = new List<Query>
                {
                    // applicationid = X AND filetype = Application
                    applicationBool
                    ,
                    //where application type and application id and user id and if files
                    userBool

                },
                MinimumShouldMatch = 1
            };

            knnFilters.Add(orBool);



            var request = new SearchRequest<ElasticSearchEntity>(indexName)
            {
                From = semanticSearchInput.skip,
                Size = semanticSearchInput.top,
                Query = new Query
                {
                    Bool = new BoolQuery
                    {
                        Should = new List<Query>
                        {
                            new MatchQuery
                            {
                                Field = Field.FromString(nameof(ElasticSearchEntity.content)),
                                Query = semanticSearchInput.query
                            }
                        },
                        Must = new List<Query>
                        {
                            new KnnQuery
                            {
                                Field = Field.FromString(nameof(ElasticSearchEntity.contentvector)),
                                QueryVector = embeddings.Value.ToArray(),
                                K = semanticSearchInput.top,
                                NumCandidates = 100,
                                Filter = knnFilters
                            }
                        }
                    }
                }
            };

            var searchResponse = await ElasticsearchClient.SearchAsync<ElasticSearchEntity>(
                request,
                cancellationToken
            );

            if (!searchResponse.IsSuccess() || !searchResponse.IsValidResponse)
            {
                Exception ex;
                if (searchResponse.TryGetOriginalException(out ex))
                {
                    logger.LogError(ex, "Elastic search error");
                }
                else
                {
                    var error = searchResponse.ElasticsearchServerError?.ToString();
                    logger.LogError($"Elastic search error : {error}");
                }

                return null;
            }

            return ToSemanticSearchOutputs(searchResponse.Documents);
        }

        protected void EnsureConfigured()
        {
            if (!isConfigured)
            {
                throw new InvalidOperationException($"{this.GetType().Name} is not configured.");
            }
        }

        protected ElasticsearchClient ElasticsearchClient
        {
            get
            {
                if (elasticsearchClient == null)
                {
                    Uri serviceEndpoint = new($"{serviceUri}");
                    var elasticSearchClientSettings = new ElasticsearchClientSettings(serviceEndpoint);
                    elasticSearchClientSettings.Authentication(new BasicAuthentication(userName, password));
                    elasticSearchClientSettings.DisableDirectStreaming(false);
                    elasticSearchClientSettings.PrettyJson(false);
                    elasticsearchClient = new ElasticsearchClient(elasticSearchClientSettings);
                }

                return elasticsearchClient;
            }
        }

        protected IReadOnlyCollection<FieldValue> CreateTermsQuery(string[] values)
        {
            var fieldValues = values.Select(v => FieldValue.String(v));
            var result = fieldValues.ToList();
            return result;
        }

        private IEnumerable<SemanticSearchOutput> ToSemanticSearchOutputs(
            IReadOnlyCollection<ElasticSearchEntity> response)
        {
            var result = new List<SemanticSearchOutput>();
            foreach (var entity in response)
            {
                var res = ToSemanticSearchOutput(entity);
                if (res != null)
                {
                    result.Add(res);
                }
            }

            return result;
        }

        private SemanticSearchOutput? ToSemanticSearchOutput(ElasticSearchEntity elasticSearchEntity)
        {
            if (elasticSearchEntity == null) return null;
            return new SemanticSearchOutput
            (
                id: elasticSearchEntity.id,
                parentid: elasticSearchEntity.parentid,
                applicationid: elasticSearchEntity.applicationid,
                datasourceid: elasticSearchEntity.datasourceid,
                userid: elasticSearchEntity.userid,
                title: elasticSearchEntity.title,
                sourceurl: elasticSearchEntity.sourceurl,
                storeidentifier: elasticSearchEntity.storeidentifier,
                bucket: elasticSearchEntity.bucket,
                filepath: elasticSearchEntity.filepath,
                content: elasticSearchEntity.content,
                keywords: elasticSearchEntity.keywords,
                sentiment: elasticSearchEntity.sentiment
            );
        }
    }
}