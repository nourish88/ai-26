using AdminBackend.Application.Services.AI;
using AdminBackend.Application.Services.Integrations;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Helpers;
using AdminBackend.Domain.Models.Services.Integrations.LiteLlm;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;

namespace AdminBackend.Infrastructure.Services.AI
{
    public class LiteLlmEmbeddingService : IEmbeddingService
    {
        private bool isConfigured = false;
        private string serviceUri = "";
        private string apiKey = "";
        private string modelName = "";
        private readonly ILogger<LiteLlmEmbeddingService> logger;
        private readonly ILiteLlmIntegration liteLlmIntegration;

        public LiteLlmEmbeddingService(ILogger<LiteLlmEmbeddingService> logger, ILiteLlmIntegration liteLlmIntegration)
        {
            this.logger = logger;
            this.liteLlmIntegration = liteLlmIntegration;
        }

        public LlmProviderTypes LlmProviderType => LlmProviderTypes.LiteLlm;

        public void Configure(IDictionary<string, string?> configuration)
        {
            if (!configuration.TryGetValue(ConfigurationConstants.EmbeddingServiceUriDictionaryKey, out serviceUri))
            {
                throw new ArgumentException("Configuration key is missing", ConfigurationConstants.EmbeddingServiceUriDictionaryKey);
            }

            if (!configuration.TryGetValue(ConfigurationConstants.EmbeddingServiceApiKeyDictionaryKey, out apiKey))
            {
                throw new ArgumentException("Configuration key is missing", ConfigurationConstants.EmbeddingServiceApiKeyDictionaryKey);
            }

            if (!configuration.TryGetValue(ConfigurationConstants.EmbeddingServiceModelNameDictionaryKey, out modelName))
            {
                throw new ArgumentException("Configuration key is missing", ConfigurationConstants.EmbeddingServiceModelNameDictionaryKey);
            }
            isConfigured = true;
        }

        public async Task<ReadOnlyMemory<float>?> GetEmbeddingAsync(string data, CancellationToken cancellationToken)
        {
            EnsureConfigured();
            Guard.Against.NullOrWhiteSpace(data, nameof(data));
            var normalizedText = data.NormalizeText();
            var embeddingRequest = new EmbeddingRequest()
            {
                apiKey = apiKey,
                baseUrl = serviceUri,
                input = normalizedText,
                model = modelName
            };

            var liteLlmEmbeddingResult = await liteLlmIntegration.EmbeddingsAsync(embeddingRequest, cancellationToken);
            if (liteLlmEmbeddingResult == null
              || liteLlmEmbeddingResult.data == null
              || liteLlmEmbeddingResult.data.Count() == 0
              || liteLlmEmbeddingResult.data[0].embedding.Length == 0)
            {
                return null;
            }
            else
            {
                return new ReadOnlyMemory<float>(liteLlmEmbeddingResult.data[0].embedding);
            }
        }

        private void EnsureConfigured()
        {
            if (!isConfigured) throw new InvalidOperationException($"{nameof(LiteLlmEmbeddingService)} is not configured");
        }
    }
}
