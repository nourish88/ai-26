using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ApplicationManagement.ApplicationMcpServerHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ApplicationManagement
{
    public class ApplicationMcpServerEndpoints : EndpointBase
    {
        public ApplicationMcpServerEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/applicationmcpservers",async (CreateApplicationMcpServerCommand command,IValidator<CreateApplicationMcpServerCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ApplicationMcpServer")
              .Produces<CreateApplicationMcpServerCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ApplicationMcpServer")
              .WithDescription("Create ApplicationMcpServer")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/applicationmcpservers", async (UpdateApplicationMcpServerCommand command, IValidator<UpdateApplicationMcpServerCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update ApplicationMcpServer")
              .Produces<UpdateApplicationMcpServerCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update ApplicationMcpServer")
              .WithDescription("Update ApplicationMcpServer")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationmcpservers/{id}", async ( ISender sender, long id) =>
            {
                var query = new ApplicationMcpServerQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationMcpServer")
              .Produces<ApplicationMcpServerQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationMcpServer")
              .WithDescription("Get ApplicationMcpServer")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationmcpservers", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ApplicationMcpServersQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationMcpServers")
              .Produces<ApplicationMcpServersQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationMcpServers")
              .WithDescription("Get ApplicationMcpServers")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/applicationmcpservers/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteApplicationMcpServerCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete ApplicationMcpServer")
              .Produces<DeleteApplicationMcpServerCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete ApplicationMcpServer")
              .WithDescription("Delete ApplicationMcpServer")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
