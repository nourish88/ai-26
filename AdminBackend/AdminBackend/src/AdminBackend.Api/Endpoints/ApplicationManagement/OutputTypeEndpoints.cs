using AdminBackend.Application.Features.ApplicationManagement.OutputTypeHandlers;
using AdminBackend.Domain.Constants;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;

namespace AdminBackend.Api.Endpoints.ApplicationManagement
{
    public class OutputTypeEndpoints : EndpointBase
    {
        public OutputTypeEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapGet("/outputtypes/{id}", async (ISender sender, OutputTypes id) =>
            {
                var query = new OutputTypeQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get OutputType")
              .Produces<OutputTypeQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get OutputType")
              .WithDescription("Get OutputType");

            app.MapGet("/outputtypes", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new OutputTypesQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get OutputTypes")
              .Produces<OutputTypesQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get OutputTypes")
              .WithDescription("Get OutputTypes");

        }
    }
}
