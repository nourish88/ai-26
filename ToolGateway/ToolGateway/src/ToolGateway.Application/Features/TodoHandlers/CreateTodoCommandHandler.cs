using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace ToolGateway.Application.Features.TodoHandlers
{

    public record CreateTodoCommand(string Title, string Description, bool IsCompleted) : ICommand<CreateTodoCommandResult>;
    public record CreateTodoCommandResult(Guid Id, string Title, string Description, bool IsCompleted);

    public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
    {
        public CreateTodoCommandValidator()
        {
            RuleFor(x => x.Title).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(50);
        }
    }

    internal class CreateTodoCommandHandler(IRepository<Domain.Entities.Todo> repository, IMapper mapper) 
        : ICommandHandler<CreateTodoCommand, CreateTodoCommandResult>
    {
        private readonly IRepository<Domain.Entities.Todo> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateTodoCommandResult> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Domain.Entities.Todo>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateTodoCommandResult>(entitiy);
            return result;
        }
    }
}
