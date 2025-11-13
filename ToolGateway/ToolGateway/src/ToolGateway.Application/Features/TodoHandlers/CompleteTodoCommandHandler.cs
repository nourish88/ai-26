using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace ToolGateway.Application.Features.TodoHandlers
{
    public record CompleteTodoCommand(Guid Id) : ICommand<CompleteTodoCommandResult>;
    public record CompleteTodoCommandResult(Guid Id, string Title, string Description, bool IsCompleted);

   
    internal class CompleteTodoCommandHandler(IRepository<Domain.Entities.Todo> repository, IMapper mapper)
        : ICommandHandler<CompleteTodoCommand, CompleteTodoCommandResult>
    {
        private readonly IRepository<Domain.Entities.Todo> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CompleteTodoCommandResult> Handle(CompleteTodoCommand request, CancellationToken cancellationToken)
        {
            var entity = await repository.FindAsync(request.Id);
            if (entity == null)
            {
                return null;
            }
            entity.IsCompleted = true;
            repository.Update(entity);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CompleteTodoCommandResult>(entity);
            return result;
        }
    }
}
