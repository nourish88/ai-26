using AdminBackend.Application.Services.AI;
using AdminBackend.Domain.Constants;

namespace AdminBackend.Infrastructure.Services.AI
{
    public class EmbeddingServiceFactory : IEmbeddingServiceFactory
    {
        private readonly IEnumerable<IEmbeddingService> embeddingServices;

        public EmbeddingServiceFactory(IEnumerable<IEmbeddingService> embeddingServices)
        {
            this.embeddingServices = embeddingServices;
        }

        public IEmbeddingService? GetEmbeddingService(LlmProviderTypes llmProviderType)
        {
            var embeddingService = embeddingServices.Where(p=>p.LlmProviderType == llmProviderType).FirstOrDefault();
            return embeddingService;
        }
    }
}
