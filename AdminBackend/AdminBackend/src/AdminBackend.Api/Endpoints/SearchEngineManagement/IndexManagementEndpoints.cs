using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.SearchEngineManagement.IndexManagementHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using MediatR;

namespace AdminBackend.Api.Endpoints.SearchEngineManagement
{
    public class IndexManagementEndpoints : EndpointBase
    {
        public IndexManagementEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/indexmanagement/createindex", async (CreateIndexCommand command, IValidator<CreateIndexCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ApplicationIndex")
              .Produces<CreateIndexCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ApplicationIndex")
              .WithDescription("Create ApplicationIndex")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
            
            app.MapDelete("/indexmanagement/{applicationId}", async (long applicationId, IValidator<DeleteIndexCommand> validator, ISender sender) =>
                {
                    var command = new DeleteIndexCommand(applicationId);
                    var val = await validator.ValidateAsync(command);
                    if (!val.IsValid)
                    {
                        return Results.BadRequest(val.GetFormattedErrors());
                    }
                    var result = await sender.Send(command);
                    return Results.Ok(result);
                })
                .WithName("Delete Application Index")
                .Produces<DeleteIndexCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Delete Application Index")
                .WithDescription("Delete Application Index")
                .RequireAuthorization(PolicyNames.AdminPolicyName);
            
            app.MapDelete("/indexmanagement/document/{fileId}", async (long fileId, IValidator<DeleteIndexedDocumentsCommand> validator, ISender sender) =>
                {
                    var command = new DeleteIndexedDocumentsCommand(fileId);
                    var val = await validator.ValidateAsync(command);
                    if (!val.IsValid)
                    {
                        return Results.BadRequest(val.GetFormattedErrors());
                    }
                    var result = await sender.Send(command);
                    return Results.Ok(result);
                })
                .WithName("Deletes file's documents from index")
                .Produces<DeleteIndexedDocumentsCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Deletes file's documents from index")
                .WithDescription("Deletes file's documents from index")
                .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
