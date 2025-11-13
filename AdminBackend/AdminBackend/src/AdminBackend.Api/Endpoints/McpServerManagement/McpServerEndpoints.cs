using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.McpServerManagement.McpServerHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.McpServerManagement
{
    public class McpServerEndpoints : EndpointBase
    {
        public McpServerEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/mcpservers", async (CreateMcpServerCommand command,IValidator<CreateMcpServerCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create McpServer")
              .Produces<CreateMcpServerCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create McpServer")
              .WithDescription("Create McpServer")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapPut("/mcpservers", async (UpdateMcpServerCommand command, IValidator<UpdateMcpServerCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update McpServer")
              .Produces<UpdateMcpServerCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update McpServer")
              .WithDescription("Update McpServer")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/mcpservers/{id}", async ( ISender sender, long id) =>
            {
                var query = new McpServerQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get McpServer")
              .Produces<McpServerQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get McpServer")
              .WithDescription("Get McpServer")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/mcpservers", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new McpServersQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get McpServers")
              .Produces<McpServersQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get McpServers")
              .WithDescription("Get McpServers")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapDelete("/mcpservers/{id}", async (ISender sender, long id) =>
            {
                var query = new DeleteMcpServerCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete McpServer")
              .Produces<DeleteMcpServerCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete McpServer")
              .WithDescription("Delete McpServer")
              .RequireAuthorization(PolicyNames.AdminPolicyName);
        }
    }
}
