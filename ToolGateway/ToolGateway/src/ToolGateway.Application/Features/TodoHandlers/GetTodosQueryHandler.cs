using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;
using ToolGateway.Application.Dtos;

namespace ToolGateway.Application.Features.TodoHandlers
{
    public record TodosQuery(PageRequest PageRequest):IQuery<TodosQueryResult>;
    public record TodosQueryResult(PageResponse<TodoDto> result);
    internal class GetTodosQueryHandler(IRepository<Domain.Entities.Todo> repository, IMapper mapper)
        : IQueryHandler<TodosQuery, TodosQueryResult>
    {
        private readonly IRepository<Domain.Entities.Todo> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<TodosQueryResult> Handle(TodosQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<TodoDto>>(entitiy);
            return new TodosQueryResult(dto);
        }
    }
}
