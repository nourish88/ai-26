using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ApplicationManagement.ApplicationEmbeddingHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ApplicationManagement
{
    public class ApplicationEmbeddingEndpoints : EndpointBase
    {
        public ApplicationEmbeddingEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/applicationembeddings",async (CreateApplicationEmbeddingCommand command,IValidator<CreateApplicationEmbeddingCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ApplicationEmbedding")
              .Produces<CreateApplicationEmbeddingCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ApplicationEmbedding")
              .WithDescription("Create ApplicationEmbedding")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/applicationembeddings", async (UpdateApplicationEmbeddingCommand command, IValidator<UpdateApplicationEmbeddingCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update ApplicationEmbedding")
              .Produces<UpdateApplicationEmbeddingCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update ApplicationEmbedding")
              .WithDescription("Update ApplicationEmbedding")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationembeddings/{id}", async ( ISender sender, long id) =>
            {
                var query = new ApplicationEmbeddingQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationEmbedding")
              .Produces<ApplicationEmbeddingQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationEmbedding")
              .WithDescription("Get ApplicationEmbedding")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationembeddings", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ApplicationEmbeddingsQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationEmbeddings")
              .Produces<ApplicationEmbeddingsQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationEmbeddings")
              .WithDescription("Get ApplicationEmbeddings")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/applicationembeddings/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteApplicationEmbeddingCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete ApplicationEmbedding")
              .Produces<DeleteApplicationEmbeddingCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete ApplicationEmbedding")
              .WithDescription("Delete ApplicationEmbedding")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
