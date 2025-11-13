using AdminBackend.Api.Policies;
using AdminBackend.Application.Features.SearchEngineManagement.SearchHandlers;
using Carter;
using Carter.ModelBinding;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AdminBackend.Api.Endpoints.SearchEngineManagement
{
    public class SearchEndpoints : EndpointBase
    {
        public SearchEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/search/semanticsearch", async ([Required][FromHeader(Name = "app-identifier")] string applicationIdentifier
                ,SemanticSearchQuery query, IValidator<SemanticSearchQuery> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(query);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("SemanticSearch")
              .Produces<SemanticSearchQueryResponse>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("SemanticSearch")
              .WithDescription("SemanticSearch")
              .RequireAuthorization(PolicyNames.ApplicationPolicyName);
        }
    }
}
