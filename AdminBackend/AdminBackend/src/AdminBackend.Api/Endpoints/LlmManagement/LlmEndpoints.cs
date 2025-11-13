using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.LlmManagement.LlmHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.LlmManagement
{
    public class LlmEndpoints : EndpointBase
    {
        public LlmEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/llms",async (CreateLlmCommand command,IValidator<CreateLlmCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create Llm")
              .Produces<CreateLlmCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Llm")
              .WithDescription("Create Llm")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/llms", async (UpdateLlmCommand command, IValidator<UpdateLlmCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update Llm")
              .Produces<UpdateLlmCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update Llm")
              .WithDescription("Update Llm")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/llms/{id}", async ( ISender sender, long id) =>
            {
                var query = new LlmQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get Llm")
              .Produces<LlmQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Llm")
              .WithDescription("Get Llm")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/llms", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new LlmsQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get Llms")
              .Produces<LlmsQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Llms")
              .WithDescription("Get Llms")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/llms/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteLlmCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete Llm")
              .Produces<DeleteLlmCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete Llm")
              .WithDescription("Delete Llm")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
