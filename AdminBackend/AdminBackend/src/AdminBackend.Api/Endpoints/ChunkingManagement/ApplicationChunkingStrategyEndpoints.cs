using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ChunkingManagement.ApplicationChunkingStrategyHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ChunkingManagement
{
    public class ApplicationChunkingStrategyEndpoints : EndpointBase
    {
        public ApplicationChunkingStrategyEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/applicationchunkingstrategies",async (CreateApplicationChunkingStrategyCommand command,IValidator<CreateApplicationChunkingStrategyCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ApplicationChunkingStrategy")
              .Produces<CreateApplicationChunkingStrategyCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ApplicationChunkingStrategy")
              .WithDescription("Create ApplicationChunkingStrategy")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/applicationchunkingstrategies", async (UpdateApplicationChunkingStrategyCommand command, IValidator<UpdateApplicationChunkingStrategyCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update ApplicationChunkingStrategy")
              .Produces<UpdateApplicationChunkingStrategyCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update ApplicationChunkingStrategy")
              .WithDescription("Update ApplicationChunkingStrategy")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationchunkingstrategies/{id}", async ( ISender sender, long id) =>
            {
                var query = new ApplicationChunkingStrategyQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationChunkingStrategy")
              .Produces<ApplicationChunkingStrategyQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationChunkingStrategy")
              .WithDescription("Get ApplicationChunkingStrategy")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationchunkingstrategies", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ApplicationChunkingStrategysQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationChunkingStrategys")
              .Produces<ApplicationChunkingStrategysQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationChunkingStrategys")
              .WithDescription("Get ApplicationChunkingStrategys")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/applicationchunkingstrategies/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteApplicationChunkingStrategyCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete ApplicationChunkingStrategy")
              .Produces<DeleteApplicationChunkingStrategyCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete ApplicationChunkingStrategy")
              .WithDescription("Delete ApplicationChunkingStrategy")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
