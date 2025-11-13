
using AdminBackend.Application.Features.SearchEngineManagement.ApplicationSearchEngineHandlers;
using AdminBackend.Application.Services.AI;
using AdminBackend.Application.Services.Integrations;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using FluentValidation;
using IdentityModel;
using MediatR;

namespace AdminBackend.Api.Endpoints
{
    public class TestEndpoints : EndpointBase
    {
        public TestEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/createembedding", async (
                LlmProviderTypes providerType,
                string apiKey,
                string baseUrl, 
                string input, 
                string model,
                IEmbeddingServiceFactory embeddingServiceFactory,CancellationToken cancellationToken=default) =>
            {
                var embeedingService = embeddingServiceFactory.GetEmbeddingService(providerType);
                var configuration = new Dictionary<string, string?>()
                {
                    {ConfigurationConstants.EmbeddingServiceUriDictionaryKey,baseUrl},
                    {ConfigurationConstants.EmbeddingServiceApiKeyDictionaryKey,apiKey},
                    {ConfigurationConstants.EmbeddingServiceModelNameDictionaryKey,model},
                };
                embeedingService.Configure(configuration);
                var embedding = await embeedingService.GetEmbeddingAsync(input, cancellationToken);
                if(embedding==null)
                {
                    return Results.Ok(new List<float>());
                }
                var result = embedding.Value.ToArray().ToList();
                return Results.Ok(result);
            })
              .WithName("Create embedding")
              .Produces<List<float>>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create emmbedding.")
              .WithDescription("Create emmbedding.");
        }
    }
}
