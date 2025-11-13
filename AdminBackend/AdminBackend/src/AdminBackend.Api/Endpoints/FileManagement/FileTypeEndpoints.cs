using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.FileManagement.FileTypeHandlers;
using AdminBackend.Domain.Constants;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.FileManagement
{
    public class FileTypeEndpoints : EndpointBase
    {
        public FileTypeEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/filetypes", async (CreateFileTypeCommand command,IValidator<CreateFileTypeCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create FileType")
              .Produces<CreateFileTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create FileType")
              .WithDescription("Create FileType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/filetypes", async (UpdateFileTypeCommand command, IValidator<UpdateFileTypeCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update FileType")
              .Produces<UpdateFileTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update FileType")
              .WithDescription("Update FileType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/filetypes/{id}", async ( ISender sender, FileTypes id) =>
            {
                var query = new FileTypeQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get FileType")
              .Produces<FileTypeQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get FileType")
              .WithDescription("Get FileType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/filetypes", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new FileTypesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get FileTypes")
              .Produces<FileTypesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get FileTypes")
              .WithDescription("Get FileTypes")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/filetypes/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteFileTypeCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete FileType")
              .Produces<DeleteFileTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete FileType")
              .WithDescription("Delete FileType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
