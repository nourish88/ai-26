using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ApplicationManagement.MemoryTypeHandlers
{
    public record MemoryTypesQuery(PageRequest PageRequest):IQuery<MemoryTypesQueryResult>;
    public record MemoryTypesQueryResult(PageResponse<MemoryTypeDto> result);
    internal class GetMemoryTypesQueryHandler(IRepository<MemoryType> repository, IMapper mapper)
        : IQueryHandler<MemoryTypesQuery, MemoryTypesQueryResult>
    {
        private readonly IRepository<MemoryType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<MemoryTypesQueryResult> Handle(MemoryTypesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<MemoryTypeDto>>(entitiy);
            return new MemoryTypesQueryResult(dto);
        }
    }
}
