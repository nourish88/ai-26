using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ApplicationManagement.AgentHandlers;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AdminBackend.Api.Endpoints.ApplicationManagement
{
    public class AgentEndpoints : EndpointBase
    {
        public AgentEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapGet("/agents/configuration", async (ISender sender, [Required][FromHeader(Name = "app-identifier")] string applicationIdentifier) =>
            {
                var query = new GetAgentConfigurationQuery(applicationIdentifier);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get Agent Configuration")
              .Produces<GetAgentConfigurationQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get OutputType")
              .WithDescription("Get OutputType")
              .RequireAuthorization(PolicyNames.ApplicationPolicyName);

            app.MapPost("/chatdetections", async ([Required][FromHeader(Name = "app-identifier")] string applicationIdentifier, CreateChatDetectionRequest request, IValidator<CreateChatDetectionCommand> validator, ISender sender) =>
            {
                var command = new CreateChatDetectionCommand(
                    applicationIdentifier,
                    request.ChatDetectionTypeId,
                    request.ThreadId,
                    request.MessageId,
                    request.UserMessage,
                    request.Sources,
                    request.Reason);
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create ChatDetection")
              .Produces<CreateChatDetectionCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create ChatDetection")
              .WithDescription("Create ChatDetection")
              .RequireAuthorization(PolicyNames.ApplicationPolicyName);

            app.MapGet("/chatdetections", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ChatDetectionsQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ChatDetections")
              .Produces<ChatDetectionsQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ChatDetections")
              .WithDescription("Get ChatDetections")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

        }
    }
}
