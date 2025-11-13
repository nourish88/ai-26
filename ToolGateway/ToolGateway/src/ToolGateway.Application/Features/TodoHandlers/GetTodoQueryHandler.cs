using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using ToolGateway.Application.Dtos;

namespace ToolGateway.Application.Features.TodoHandlers
{
    public record TodoQuery(Guid Id):IQuery<TodoQueryResult>;
    public record TodoQueryResult(TodoDto result);
    internal class GetTodoQueryHandler(IRepository<Domain.Entities.Todo> repository, IMapper mapper)
        : IQueryHandler<TodoQuery, TodoQueryResult>
    {
        private readonly IRepository<Domain.Entities.Todo> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<TodoQueryResult> Handle(TodoQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<TodoDto>(entitiy);
            return new TodoQueryResult(dto);
        }
    }
}
