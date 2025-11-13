using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.LlmManagement.LlmProviderHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.LlmManagement
{
    public class LlmProviderEndpoints : EndpointBase
    {
        public LlmProviderEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/llmproviders",async (CreateLlmProviderCommand command,IValidator<CreateLlmProviderCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create LlmProvider")
              .Produces<CreateLlmProviderCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create LlmProvider")
              .WithDescription("Create LlmProvider")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/llmproviders", async (UpdateLlmProviderCommand command, IValidator<UpdateLlmProviderCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update LlmProvider")
              .Produces<UpdateLlmProviderCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update LlmProvider")
              .WithDescription("Update LlmProvider")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/llmproviders/{id}", async ( ISender sender, long id) =>
            {
                var query = new LlmProviderQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get LlmProvider")
              .Produces<LlmProviderQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get LlmProvider")
              .WithDescription("Get LlmProvider")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/llmproviders", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new LlmProvidersQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get LlmProviders")
              .Produces<LlmProvidersQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get LlmProviders")
              .WithDescription("Get LlmProviders")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/llmproviders/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteLlmProviderCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete LlmProvider")
              .Produces<DeleteLlmProviderCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete LlmProvider")
              .WithDescription("Delete LlmProvider")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
