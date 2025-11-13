using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.SearchEngineManagement.SearchEngineTypeHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.SearchEngineManagement
{
    public class SearchEngineTypeEndpoints : EndpointBase
    {
        public SearchEngineTypeEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/searchenginetypes",async (CreateSearchEngineTypeCommand command,IValidator<CreateSearchEngineTypeCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create SearchEngineType")
              .Produces<CreateSearchEngineTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create SearchEngineType")
              .WithDescription("Create SearchEngineType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/searchenginetypes", async (UpdateSearchEngineTypeCommand command, IValidator<UpdateSearchEngineTypeCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update SearchEngineType")
              .Produces<UpdateSearchEngineTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update SearchEngineType")
              .WithDescription("Update SearchEngineType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/searchenginetypes/{id}", async ( ISender sender, long id) =>
            {
                var query = new SearchEngineTypeQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get SearchEngineType")
              .Produces<SearchEngineTypeQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get SearchEngineType")
              .WithDescription("Get SearchEngineType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/searchenginetypes", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new SearchEngineTypesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get SearchEngineTypes")
              .Produces<SearchEngineTypesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get SearchEngineTypes")
              .WithDescription("Get SearchEngineTypes")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/searchenginetypes/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteSearchEngineTypeCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete SearchEngineType")
              .Produces<DeleteSearchEngineTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete SearchEngineType")
              .WithDescription("Delete SearchEngineType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
