using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.ApplicationManagement.ApplicationTypeHandlers;
using AdminBackend.Domain.Constants;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ApplicationManagement
{
    public class ApplicationTypeEndpoints : EndpointBase
    {
        public ApplicationTypeEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapGet("/applicationtypes/{id}", async (ISender sender, ApplicationTypes id) =>
            {
                var query = new ApplicationTypeQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationType")
              .Produces<ApplicationTypeQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationType")
              .WithDescription("Get ApplicationType")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

            app.MapGet("/applicationtypes", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new ApplicationTypesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get ApplicationTypes")
              .Produces<ApplicationTypesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get ApplicationTypes")
              .WithDescription("Get ApplicationTypes")
              .RequireAuthorization(PolicyNames.AdminPolicyName);

        }
    }
}
