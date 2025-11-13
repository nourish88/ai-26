using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.McpServerManagement.McpServerHandlers
{
    public record McpServerQuery(long Id):IQuery<McpServerQueryResult>;
    public record McpServerQueryResult(McpServerDto result);
    internal class GetMcpServerQueryHandler(IRepository<McpServer> repository, IMapper mapper)
        : IQueryHandler<McpServerQuery, McpServerQueryResult>
    {
        private readonly IRepository<McpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<McpServerQueryResult> Handle(McpServerQuery request, CancellationToken cancellationToken)
        {
            var entity = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<McpServerDto>(entity);
            return new McpServerQueryResult(dto);
        }
    }
}
