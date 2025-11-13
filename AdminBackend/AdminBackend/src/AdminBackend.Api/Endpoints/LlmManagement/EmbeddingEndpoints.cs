using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.LlmManagement.EmbeddingHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.LlmManagement
{
    public class EmbeddingEndpoints : EndpointBase
    {
        public EmbeddingEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/embeddings",async (CreateEmbeddingCommand command,IValidator<CreateEmbeddingCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create Embedding")
              .Produces<CreateEmbeddingCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Embedding")
              .WithDescription("Create Embedding")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/embeddings", async (UpdateEmbeddingCommand command, IValidator<UpdateEmbeddingCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update Embedding")
              .Produces<UpdateEmbeddingCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update Embedding")
              .WithDescription("Update Embedding")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/embeddings/{id}", async ( ISender sender, long id) =>
            {
                var query = new EmbeddingQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get Embedding")
              .Produces<EmbeddingQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Embedding")
              .WithDescription("Get Embedding")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/embeddings", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new EmbeddingsQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get Embeddings")
              .Produces<EmbeddingsQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Embeddings")
              .WithDescription("Get Embeddings")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/embeddings/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteEmbeddingCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete Embedding")
              .Produces<DeleteEmbeddingCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete Embedding")
              .WithDescription("Delete Embedding")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
