using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.FileManagement.IngestionStatusTypeHandlers;
using AdminBackend.Domain.Constants;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.FileManagement
{
    public class IngestionStatusTypeEndpoints : EndpointBase
    {
        public IngestionStatusTypeEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/ingestionstatustypes", async (CreateIngestionStatusTypeCommand command,IValidator<CreateIngestionStatusTypeCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create IngestionStatusType")
              .Produces<CreateIngestionStatusTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create IngestionStatusType")
              .WithDescription("Create IngestionStatusType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/ingestionstatustypes", async (UpdateIngestionStatusTypeCommand command, IValidator<UpdateIngestionStatusTypeCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update IngestionStatusType")
              .Produces<UpdateIngestionStatusTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update IngestionStatusType")
              .WithDescription("Update IngestionStatusType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/ingestionstatustypes/{id}", async ( ISender sender, IngestionStatusTypes id) =>
            {
                var query = new IngestionStatusTypeQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get IngestionStatusType")
              .Produces<IngestionStatusTypeQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get IngestionStatusType")
              .WithDescription("Get IngestionStatusType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/ingestionstatustypes", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new IngestionStatusTypesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get IngestionStatusTypes")
              .Produces<IngestionStatusTypesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get IngestionStatusTypes")
              .WithDescription("Get IngestionStatusTypes")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/ingestionstatustypes/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteIngestionStatusTypeCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete IngestionStatusType")
              .Produces<DeleteIngestionStatusTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete IngestionStatusType")
              .WithDescription("Delete IngestionStatusType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
