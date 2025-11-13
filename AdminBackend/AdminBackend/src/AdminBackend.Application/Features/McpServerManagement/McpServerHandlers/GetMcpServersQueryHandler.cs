using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.McpServerManagement.McpServerHandlers
{
    public record McpServersQuery(PageRequest PageRequest):IQuery<McpServersQueryResult>;
    public record McpServersQueryResult(PageResponse<McpServerDto> result);
    internal class GetMcpServersQueryHandler(IRepository<McpServer> repository, IMapper mapper)
        : IQueryHandler<McpServersQuery, McpServersQueryResult>
    {
        private readonly IRepository<McpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<McpServersQueryResult> Handle(McpServersQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<McpServerDto>>(entitiy);
            return new McpServersQueryResult(dto);
        }
    }
}
