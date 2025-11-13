using Carter;
using Carter.ModelBinding;
using FluentValidation;
using Juga.Data.Paging;
using MediatR;
using ToolGateway.Application.Features.TodoHandlers;

namespace ToolGateway.Api.Endpoints
{
    public class TodoEndpoints : EndpointBase
    {
        public TodoEndpoints(IConfiguration configuration) : base(configuration)
        {
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/todos",async (CreateTodoCommand command,IValidator<CreateTodoCommand> validator,ISender sender)=>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Create Todo")
              .Produces<CreateTodoCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Create Todo")
              .WithDescription("Create Todo");

            app.MapPut("/todos", async (UpdateTodoCommand command, IValidator<UpdateTodoCommand> validator, ISender sender) =>
            {
                var val = await validator.ValidateAsync(command);
                if (!val.IsValid)
                {
                    return Results.BadRequest(val.GetFormattedErrors());
                }
                var result = await sender.Send(command);
                return Results.Ok(result);
            })
              .WithName("Update Todo")
              .Produces<UpdateTodoCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Update Todo")
              .WithDescription("Update Todo");

            app.MapGet("/todos/{id}", async ( ISender sender, Guid id) =>
            {
                var query = new TodoQuery(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get Todo")
              .Produces<TodoQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Todo")
              .WithDescription("Get Todo");

            app.MapGet("/todos", async ([AsParameters] PageRequest request, ISender sender) =>
            {
                var query = new TodosQuery(request);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Get Todos")
              .Produces<TodosQueryResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Get Todos")
              .WithDescription("Get Todos");

            app.MapDelete("/todos/{id}", async (ISender sender, Guid id) =>
            {
                var query = new DeleteTodoCommand(id);
                var result = await sender.Send(query);
                return Results.Ok(result);
            })
              .WithName("Delete Todo")
              .Produces<DeleteTodoCommandResult>(StatusCodes.Status200OK)
              .ProducesValidationProblem(StatusCodes.Status400BadRequest)
              .WithSummary("Delete Todo")
              .WithDescription("Delete Todo");
        }
    }
}
