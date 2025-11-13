using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.SearchEngineManagement.ApplicationSearchEngineHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.SearchEngineManagement
{
    public class ApplicationSearchEngineEndpoints : EndpointBase
    {
        public ApplicationSearchEngineEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/applicationsearchengines",async (CreateApplicationSearchEngineCommand command,IValidator<CreateApplicationSearchEngineCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ApplicationSearchEngine")
              .Produces<CreateApplicationSearchEngineCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ApplicationSearchEngine")
              .WithDescription("Create ApplicationSearchEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/applicationsearchengines", async (UpdateApplicationSearchEngineCommand command, IValidator<UpdateApplicationSearchEngineCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update ApplicationSearchEngine")
              .Produces<UpdateApplicationSearchEngineCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update ApplicationSearchEngine")
              .WithDescription("Update ApplicationSearchEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationsearchengines/{id}", async ( ISender sender, long id) =>
            {
                var query = new ApplicationSearchEngineQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationSearchEngine")
              .Produces<ApplicationSearchEngineQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationSearchEngine")
              .WithDescription("Get ApplicationSearchEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationsearchengines", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ApplicationSearchEnginesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationSearchEngines")
              .Produces<ApplicationSearchEnginesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationSearchEngines")
              .WithDescription("Get ApplicationSearchEngines")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/applicationsearchengines/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteApplicationSearchEngineCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete ApplicationSearchEngine")
              .Produces<DeleteApplicationSearchEngineCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete ApplicationSearchEngine")
              .WithDescription("Delete ApplicationSearchEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
