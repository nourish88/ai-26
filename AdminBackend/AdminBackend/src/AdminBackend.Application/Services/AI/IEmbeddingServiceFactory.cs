using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Services.AI
{
    public interface IEmbeddingServiceFactory
    {
        public IEmbeddingService? GetEmbeddingService(LlmProviderTypes llmProviderType);
    }
}
