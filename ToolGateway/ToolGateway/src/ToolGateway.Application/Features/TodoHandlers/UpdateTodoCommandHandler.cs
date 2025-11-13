using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace ToolGateway.Application.Features.TodoHandlers
{
    public record UpdateTodoCommand(Guid Id, string Title, string Description, bool IsCompleted) : ICommand<UpdateTodoCommandResult>;
    public record UpdateTodoCommandResult(Guid Id, string Title, string Description, bool IsCompleted) ;

    public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
    {
        public UpdateTodoCommandValidator()
        {
            RuleFor(x => x.Title).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(50);
        }
    }
    internal class UpdateTodoCommandHandler(IRepository<Domain.Entities.Todo> repository, IMapper mapper)
        : ICommandHandler<UpdateTodoCommand, UpdateTodoCommandResult>
    {
        private readonly IRepository<Domain.Entities.Todo> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateTodoCommandResult> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Domain.Entities.Todo>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateTodoCommandResult>(entitiy);
            return result;
        }
    }
}
