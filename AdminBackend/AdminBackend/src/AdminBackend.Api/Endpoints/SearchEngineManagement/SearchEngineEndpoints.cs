using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.SearchEngineManagement.SearchEngineHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.SearchEngineManagement
{
    public class SearchEngineEndpoints : EndpointBase
    {
        public SearchEngineEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/searchengines",async (CreateSearchEngineCommand command,IValidator<CreateSearchEngineCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create SearchEngine")
              .Produces<CreateSearchEngineCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create SearchEngine")
              .WithDescription("Create SearchEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/searchengines", async (UpdateSearchEngineCommand command, IValidator<UpdateSearchEngineCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update SearchEngine")
              .Produces<UpdateSearchEngineCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update SearchEngine")
              .WithDescription("Update SearchEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/searchengines/{id}", async ( ISender sender, long id) =>
            {
                var query = new SearchEngineQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get SearchEngine")
              .Produces<SearchEngineQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get SearchEngine")
              .WithDescription("Get SearchEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/searchengines", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new SearchEnginesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get SearchEngines")
              .Produces<SearchEnginesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get SearchEngines")
              .WithDescription("Get SearchEngines")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/searchengines/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteSearchEngineCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete SearchEngine")
              .Produces<DeleteSearchEngineCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete SearchEngine")
              .WithDescription("Delete SearchEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
