using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.FileManagement.FileStoreHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.FileManagement
{
    public class FileStoreEndpoints : EndpointBase
    {
        public FileStoreEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/fileStores", async (CreateFileStoreCommand command,IValidator<CreateFileStoreCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create FileStore")
              .Produces<CreateFileStoreCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create FileStore")
              .WithDescription("Create FileStore")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/fileStores", async (UpdateFileStoreCommand command, IValidator<UpdateFileStoreCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update FileStore")
              .Produces<UpdateFileStoreCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update FileStore")
              .WithDescription("Update FileStore")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/fileStores/{id}", async ( ISender sender, long id) =>
            {
                var query = new FileStoreQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get FileStore")
              .Produces<FileStoreQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get FileStore")
              .WithDescription("Get FileStore")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/fileStores", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new FileStoresQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get FileStores")
              .Produces<FileStoresQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get FileStores")
              .WithDescription("Get FileStores")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/fileStores/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteFileStoreCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete FileStore")
              .Produces<DeleteFileStoreCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete FileStore")
              .WithDescription("Delete FileStore")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
