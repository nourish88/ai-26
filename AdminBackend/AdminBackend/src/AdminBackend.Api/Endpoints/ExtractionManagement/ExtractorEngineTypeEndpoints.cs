using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ExtractionManagement.ExtractorEngineTypeHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ExtractionManagement
{
    public class ExtractorEngineTypeEndpoints : EndpointBase
    {
        public ExtractorEngineTypeEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/extractorenginetypes",async (CreateExtractorEngineTypeCommand command,IValidator<CreateExtractorEngineTypeCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ExtractorEngineType")
              .Produces<CreateExtractorEngineTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ExtractorEngineType")
              .WithDescription("Create ExtractorEngineType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/extractorenginetypes", async (UpdateExtractorEngineTypeCommand command, IValidator<UpdateExtractorEngineTypeCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update ExtractorEngineType")
              .Produces<UpdateExtractorEngineTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update ExtractorEngineType")
              .WithDescription("Update ExtractorEngineType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/extractorenginetypes/{id}", async ( ISender sender, long id) =>
            {
                var query = new ExtractorEngineTypeQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ExtractorEngineType")
              .Produces<ExtractorEngineTypeQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ExtractorEngineType")
              .WithDescription("Get ExtractorEngineType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/extractorenginetypes", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ExtractorEngineTypesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ExtractorEngineTypes")
              .Produces<ExtractorEngineTypesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ExtractorEngineTypes")
              .WithDescription("Get ExtractorEngineTypes")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/extractorenginetypes/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteExtractorEngineTypeCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete ExtractorEngineType")
              .Produces<DeleteExtractorEngineTypeCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete ExtractorEngineType")
              .WithDescription("Delete ExtractorEngineType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
