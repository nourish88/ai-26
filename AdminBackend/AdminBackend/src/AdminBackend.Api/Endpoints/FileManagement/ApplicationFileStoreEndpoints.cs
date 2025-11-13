using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.FileManagement.ApplicationFileStoreHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.FileManagement
{
    public class ApplicationFileStoreEndpoints : EndpointBase
    {
        public ApplicationFileStoreEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/applicationfilestores", async (CreateApplicationFileStoreCommand command,IValidator<CreateApplicationFileStoreCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ApplicationFileStore")
              .Produces<CreateApplicationFileStoreCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ApplicationFileStore")
              .WithDescription("Create ApplicationFileStore")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/applicationfilestores", async (UpdateApplicationFileStoreCommand command, IValidator<UpdateApplicationFileStoreCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update ApplicationFileStore")
              .Produces<UpdateApplicationFileStoreCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update ApplicationFileStore")
              .WithDescription("Update ApplicationFileStore")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationfilestores/{id}", async ( ISender sender, long id) =>
            {
                var query = new ApplicationFileStoreQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationFileStore")
              .Produces<ApplicationFileStoreQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationFileStore")
              .WithDescription("Get ApplicationFileStore")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationfilestores", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ApplicationFileStoresQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationFileStores")
              .Produces<ApplicationFileStoresQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationFileStores")
              .WithDescription("Get ApplicationFileStores")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/applicationfilestores/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteApplicationFileStoreCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete ApplicationFileStore")
              .Produces<DeleteApplicationFileStoreCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete ApplicationFileStore")
              .WithDescription("Delete ApplicationFileStore")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
