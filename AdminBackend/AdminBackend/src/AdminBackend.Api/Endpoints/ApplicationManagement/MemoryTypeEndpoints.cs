using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ApplicationManagement.MemoryTypeHandlers;
using AdminBackend.Domain.Constants;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ApplicationManagement
{
    public class MemoryTypeEndpoints : EndpointBase
    {
        public MemoryTypeEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapGet("/memorytypes/{id}", async (ISender sender, MemoryTypes id) =>
            {
                var query = new MemoryTypeQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get MemoryType")
              .Produces<MemoryTypeQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get MemoryType")
              .WithDescription("Get MemoryType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/memorytypes", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new MemoryTypesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get MemoryTypes")
              .Produces<MemoryTypesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get MemoryTypes")
              .WithDescription("Get MemoryTypes")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

        }
    }
}
