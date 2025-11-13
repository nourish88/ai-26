using AdminBackend.Application.Services.Integrations;
using AdminBackend.Domain.Models.Services.Integrations.LiteLlm;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace AdminBackend.Infrastructure.Services.Integrations
{
    public class LiteLlmIntegration : ILiteLlmIntegration
    {
        private readonly ILogger<LiteLlmIntegration> logger;
        private readonly IHttpClientFactory httpClientFactory;
        private const string embbedingUri = "v1/embeddings";

        public LiteLlmIntegration(ILogger<LiteLlmIntegration> logger, IHttpClientFactory httpClientFactory)
        {
            this.logger = logger;
            this.httpClientFactory = httpClientFactory;
        }


        public async Task<EmbeddingResponse?> EmbeddingsAsync(EmbeddingRequest request, CancellationToken cancellationToken)
        {
            using (var client = CreateHttpClient(request.baseUrl, request.apiKey))
            {
                try
                {
                    var requestItem = new LiteLlmEmbeddingRequest
                    {
                        model = request.model,
                        input = request.input
                    };
                    var jsonContent = JsonContent.Create<LiteLlmEmbeddingRequest>(requestItem);
                    await jsonContent.LoadIntoBufferAsync();
                    var postResult = await client.PostAsync(embbedingUri, jsonContent, cancellationToken);

                    if (postResult != null && postResult.IsSuccessStatusCode)
                    {
                        var result = await postResult.Content.ReadFromJsonAsync<EmbeddingResponse>(cancellationToken);
                        return result;
                    }
                    else
                    {
                        var content = postResult == null ? "Unknown" : await postResult.Content.ReadAsStringAsync(cancellationToken);
                        logger.LogError($"LiteLlm({client.BaseAddress}{embbedingUri}) embedding error for model: {request.model}, input: {request.input}, response statuscode: {postResult?.StatusCode}, response: {content}");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"LiteLlm({client.BaseAddress}{embbedingUri}) embedding error for model {request.model}, input: {request.input}");
                    return null;
                }
            }
        }

        private HttpClient CreateHttpClient(string baseUrl, string apiKey)
        {
            var client = httpClientFactory.CreateClient("LiteLlmClient");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            client.BaseAddress = new Uri(baseUrl);
            return client;
        }
    }
}
