using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationMcpServerHandlers
{
    public record ApplicationMcpServersQuery(PageRequest PageRequest):IQuery<ApplicationMcpServersQueryResult>;
    public record ApplicationMcpServersQueryResult(PageResponse<ApplicationMcpServerDto> result);
    internal class GetApplicationMcpServersQueryHandler(IRepository<ApplicationMcpServer> repository, IMapper mapper)
        : IQueryHandler<ApplicationMcpServersQuery, ApplicationMcpServersQueryResult>
    {
        private readonly IRepository<ApplicationMcpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationMcpServersQueryResult> Handle(ApplicationMcpServersQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ApplicationMcpServerDto>>(entitiy);
            return new ApplicationMcpServersQueryResult(dto);
        }
    }
}
