using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ExtractionManagement.ApplicationExtractorEngineHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ExtractionManagement
{
    public class ApplicationExtractorEngineEndpoints : EndpointBase
    {
        public ApplicationExtractorEngineEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/applicationextractorengine",async (CreateApplicationExtractorEngineCommand command,IValidator<CreateApplicationExtractorEngineCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ApplicationExtractorEngine")
              .Produces<CreateApplicationExtractorEngineCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ApplicationExtractorEngine")
              .WithDescription("Create ApplicationExtractorEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/applicationextractorengine", async (UpdateApplicationExtractorEngineCommand command, IValidator<UpdateApplicationExtractorEngineCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update ApplicationExtractorEngine")
              .Produces<UpdateApplicationExtractorEngineCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update ApplicationExtractorEngine")
              .WithDescription("Update ApplicationExtractorEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationextractorengine/{id}", async ( ISender sender, long id) =>
            {
                var query = new ApplicationExtractorEngineQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationExtractorEngine")
              .Produces<ApplicationExtractorEngineQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationExtractorEngine")
              .WithDescription("Get ApplicationExtractorEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationextractorengine", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ApplicationExtractorEnginesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationExtractorEngines")
              .Produces<ApplicationExtractorEnginesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationExtractorEngines")
              .WithDescription("Get ApplicationExtractorEngines")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/applicationextractorengine/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteApplicationExtractorEngineCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete ApplicationExtractorEngine")
              .Produces<DeleteApplicationExtractorEngineCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete ApplicationExtractorEngine")
              .WithDescription("Delete ApplicationExtractorEngine")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
