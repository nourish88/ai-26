using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace ToolGateway.Application.Features.TodoHandlers
{

    public record DeleteTodoCommand(Guid Id):ICommand<DeleteTodoCommandResult>;
    public record DeleteTodoCommandResult(bool result);    

    internal class DeleteTodoCommandHandler(IRepository<Domain.Entities.Todo> repository, IMapper mapper) 
        : ICommandHandler<DeleteTodoCommand, DeleteTodoCommandResult>
    {
        private readonly IRepository<Domain.Entities.Todo> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteTodoCommandResult> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteTodoCommandResult(true);
        }
    }
}
