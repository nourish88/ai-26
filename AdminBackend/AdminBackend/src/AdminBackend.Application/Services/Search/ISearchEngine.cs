using AdminBackend.Application.Services.AI;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Models.Services.Search;

namespace AdminBackend.Application.Services.Search
{
    public interface ISearchEngine
    {
        SearchEngineTypes SearchEngineType { get; }
        string[] SupportedIndexIdentifiers { get; }
        void Configure(IDictionary<string, string?> configuration);
        Task<bool> CreateIndexAsync(IIndexDefinition indexDefinition, string indexName, CancellationToken cancellationToken = default);
        Task<bool> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
        Task<bool> DeleteDocumentsByParentIdAsync(string parentId, string indexName,
            CancellationToken cancellationToken = default);
        Task<bool> BulkInsertAsync(IReadOnlyList<ElasticSearchEntity> entities, string indexName,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<SemanticSearchOutput>?> SemanticSearchAsync(IEmbeddingService embeddingService, SemanticSearchInput semanticSearchInput, string indexName, CancellationToken cancellationToken = default);
    }
}
