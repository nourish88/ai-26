using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Services.AI
{
    public interface IEmbeddingService
    {
        public LlmProviderTypes LlmProviderType { get; }
        public Task<ReadOnlyMemory<float>?> GetEmbeddingAsync(string data, CancellationToken cancellationToken);
        public void Configure(IDictionary<string, string?> configuration);
    }
}
