using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ApplicationManagement.ApplicationLlmHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ApplicationManagement
{
    public class ApplicationLlmEndpoints : EndpointBase
    {
        public ApplicationLlmEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/applicationllms",async (CreateApplicationLlmCommand command,IValidator<CreateApplicationLlmCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ApplicationLlm")
              .Produces<CreateApplicationLlmCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ApplicationLlm")
              .WithDescription("Create ApplicationLlm")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/applicationllms", async (UpdateApplicationLlmCommand command, IValidator<UpdateApplicationLlmCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update ApplicationLlm")
              .Produces<UpdateApplicationLlmCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update ApplicationLlm")
              .WithDescription("Update ApplicationLlm")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationllms/{id}", async ( ISender sender, long id) =>
            {
                var query = new ApplicationLlmQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationLlm")
              .Produces<ApplicationLlmQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationLlm")
              .WithDescription("Get ApplicationLlm")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationllms", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ApplicationLlmsQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationLlms")
              .Produces<ApplicationLlmsQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationLlms")
              .WithDescription("Get ApplicationLlms")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/applicationllms/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteApplicationLlmCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete ApplicationLlm")
              .Produces<DeleteApplicationLlmCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete ApplicationLlm")
              .WithDescription("Delete ApplicationLlm")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
