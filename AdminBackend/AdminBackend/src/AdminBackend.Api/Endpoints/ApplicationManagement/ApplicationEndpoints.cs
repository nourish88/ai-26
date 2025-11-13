using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ApplicationManagement.ApplicationHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ApplicationManagement
{
    public class ApplicationEndpoints : EndpointBase
    {
        public ApplicationEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/applications", async (CreateApplicationCommand command, IValidator<CreateApplicationCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create Application")
              .Produces<CreateApplicationCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Application")
              .WithDescription("Create Application");
              //.RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/applications", async (UpdateApplicationCommand command, IValidator<UpdateApplicationCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update Application")
              .Produces<UpdateApplicationCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update Application")
              .WithDescription("Update Application")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applications/{id}", async ( ISender sender, long id) =>
            {
                var query = new ApplicationQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get Application")
              .Produces<ApplicationQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Application")
              .WithDescription("Get Application")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applications", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ApplicationsQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get Applications")
              .Produces<ApplicationsQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Applications")
              .WithDescription("Get Applications")
              .RequireAuthorization(PolicyNames.EitherPolicy);

            app.MapDelete("/applications/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteApplicationCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete Application")
              .Produces<DeleteApplicationCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete Application")
              .WithDescription("Delete Application")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
