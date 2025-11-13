using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.FileManagement.FileHandlers;
using AdminBackend.Domain.Constants;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AdminBackend.Api.Endpoints.FileManagement
{
    public class FileEndpoints : EndpointBase
    {
        public FileEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/files",
                    async (CreateFileCommand command, IValidator<CreateFileCommand> validator, ISender sender) =>
                    {
                        var val = await validator.ValidateAsync(command);
                        if (!val.IsValid)
                        {
                            return Results.BadRequest(val.GetFormattedErrors());
                        }

                        var result = await sender.Send(command);
                        return Results.Ok(result);
                    })
                .WithName("Create File")
                .Produces<CreateFileCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Create File")
                .WithDescription("Create File")
                .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/files",
                    async (UpdateFileCommand command, IValidator<UpdateFileCommand> validator, ISender sender) =>
                    {
                        var val = await validator.ValidateAsync(command);
                        if (!val.IsValid)
                        {
                            return Results.BadRequest(val.GetFormattedErrors());
                        }

                        var result = await sender.Send(command);
                        return Results.Ok(result);
                    })
                .WithName("Update File")
                .Produces<UpdateFileCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Update File")
                .WithDescription("Update File")
                .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/files/{id}", async (ISender sender, long id) =>
                {
                    var query = new FileQuery(id);
                    var result = await sender.Send(query);
                    return Results.Ok(result);
                })
                .WithName("Get File")
                .Produces<FileQueryResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get File")
                .WithDescription("Get File")
                .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/files", async ([AsParameters] PageRequest request, ISender sender) =>
                {
                    var query = new FilesQuery(request);
                    var result = await sender.Send(query);
                    return Results.Ok(result);
                })
                .WithName("Get Files")
                .Produces<UserFilesQueryResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Files")
                .WithDescription("Get Files")
                .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/files/user", async ([Required][FromHeader(Name = "app-identifier")] string applicationIdentifier, ISender sender) =>
            {
                var query = new UserFilesQuery(applicationIdentifier);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
                .WithName("Get User Files For Application")
                .Produces<FilesQueryResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get User Files For Application")
                .WithDescription("Get User Files For Application")
                .RequireAuthorization(PolicyNames.ApplicationPolicyName);

            app.MapGet("/files/application", async ([AsParameters] ApplicationFilesPagedRequest request, ISender sender) =>
            {
                var query = new ApplicationFilesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
                .WithName("Get Application Files")
                .Produces<ApplicationFilesQueryResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Application Files")
                .WithDescription("Get Application Files")
                .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/files/{id}", async (ISender sender, long id) =>
                {
                    var query = new DeleteFileCommand(id);
                    var result = await sender.Send(query);
                    return Results.Ok(result);
                })
                .WithName("Delete File")
                .Produces<DeleteFileCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Delete File")
                .WithDescription("Delete File")
                .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/files/status/{id}",
                    async (long id, ISender sender) =>
                    {
                        var query = new FileStatusQuery(id);
                        var result = await sender.Send(query);
                        return Results.Ok(result);
                    })
                .WithName("Get File Status")
                .Produces<FileStatusQueryResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get File Status")
                .WithDescription("Get File Status");

            app.MapPut("/files/status",
                    async (UpdateFileStatusCommand command, IValidator<UpdateFileStatusCommand> validator, ISender sender) =>
                    {
                        var val = await validator.ValidateAsync(command);
                        if (!val.IsValid)
                        {
                            return Results.BadRequest(val.GetFormattedErrors());
                        }

                        var result = await sender.Send(command);
                        return Results.Ok(result);
                    })
                .WithName("Change File Status")
                .Produces<UpdateFileStatusCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Change File Status")
                .WithDescription("Change File Status");

            app.MapPut("/files/status/error",
                    async (UpdateFileErrorStatusCommand command, IValidator<UpdateFileErrorStatusCommand> validator, ISender sender) =>
                    {
                        var val = await validator.ValidateAsync(command);
                        if (!val.IsValid)
                        {
                            return Results.BadRequest(val.GetFormattedErrors());
                        }

                        var result = await sender.Send(command);
                        return Results.Ok(result);
                    })
                .WithName("Set File Status To ProcessingFailed")
                .Produces<UpdateFileErrorStatusCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Set File Status To ProcessingFailed")
                .WithDescription("Set File Status To ProcessingFailed");

            app.MapPost("/files/user/storage", async (
                    [Required][FromHeader(Name = "app-identifier")]
                    string applicationIdentifier,
                    [FromForm] UploadRequest request,
                    IValidator<UploadFilesCommand> validator,
                    ISender sender) =>
                {
                    var command = new UploadFilesCommand(applicationIdentifier, FileTypes.Personal, request.Files);
                    var val = await validator.ValidateAsync(command);
                    if (!val.IsValid)
                    {
                        return Results.BadRequest(val.GetFormattedErrors());
                    }
                    var result = await sender.Send(command);
                    return Results.Ok(result);
                })
                .WithName("User uploads user file for a specific application")
                .Produces<UploadFilesCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Uploads user files in batch and store them in file storage for appilcation")
                .DisableAntiforgery()
                .WithDescription(
                    "Uploads files in batch and store them in file storage and sends them to processing pipeline")
                .RequireAuthorization(PolicyNames.ApplicationPolicyName);

            app.MapDelete("/files/user/{id}", async (
                ISender sender,
                long id,
                [Required][FromHeader(Name = "app-identifier")] string applicationIdentifier
                    ) =>
            {
                var query = new DeleteUserFileCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
                .WithName("Delete User File")
                .Produces<DeleteUserFileCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Delete User File")
                .WithDescription("Delete User File")
                .RequireAuthorization(PolicyNames.ApplicationPolicyName);

            app.MapPost("/files/application/storage", async (                  
                    
                    [FromForm] UploadApplicationFileRequest request,
                    IValidator<UploadFilesCommand> validator,
                    ISender sender) =>
            {
                var command = new UploadFilesCommand(request.ApplicationIdentifier, FileTypes.Application, request.Files);
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
                .WithName("Admin uploads application file for a specific application")
                .Produces<UploadFilesCommandResult>(StatusCodes.Status200OK)
                .ProducesValidationProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Uploads application files in batch and store them in file storage for appilcation")
                .DisableAntiforgery()
                .WithDescription(
                    "Uploads files in batch and store them in file storage and sends them to processing pipeline")
                .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}