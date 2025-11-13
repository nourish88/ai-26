using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ChunkingManagement.ChunkingStrategyHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ChunkingManagement
{
    public class ChunkingStrategyEndpoints : EndpointBase
    {
        public ChunkingStrategyEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/chunkingstrategies",async (CreateChunkingStrategyCommand command,IValidator<CreateChunkingStrategyCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ChunkingStrategy")
              .Produces<CreateChunkingStrategyCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ChunkingStrategy")
              .WithDescription("Create ChunkingStrategy")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/chunkingstrategies", async (UpdateChunkingStrategyCommand command, IValidator<UpdateChunkingStrategyCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update ChunkingStrategy")
              .Produces<UpdateChunkingStrategyCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update ChunkingStrategy")
              .WithDescription("Update ChunkingStrategy")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/chunkingstrategies/{id}", async ( ISender sender, long id) =>
            {
                var query = new ChunkingStrategyQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ChunkingStrategy")
              .Produces<ChunkingStrategyQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ChunkingStrategy")
              .WithDescription("Get ChunkingStrategy")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/chunkingstrategies", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ChunkingStrategysQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ChunkingStrategys")
              .Produces<ChunkingStrategysQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ChunkingStrategys")
              .WithDescription("Get ChunkingStrategys")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/chunkingstrategies/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteChunkingStrategyCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete ChunkingStrategy")
              .Produces<DeleteChunkingStrategyCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete ChunkingStrategy")
              .WithDescription("Delete ChunkingStrategy")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
