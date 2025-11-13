using AdminBackend.Domain.Models.Services.Integrations.LiteLlm;

namespace AdminBackend.Application.Services.Integrations
{
    public interface ILiteLlmIntegration
    {
        public Task<EmbeddingResponse?> EmbeddingsAsync(EmbeddingRequest request, CancellationToken cancellationToken);
    }
}
