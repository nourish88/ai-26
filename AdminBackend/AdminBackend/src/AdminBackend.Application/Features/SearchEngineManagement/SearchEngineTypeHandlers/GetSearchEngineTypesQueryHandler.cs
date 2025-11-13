using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineTypeHandlers
{
    public record SearchEngineTypesQuery(PageRequest PageRequest):IQuery<SearchEngineTypesQueryResult>;
    public record SearchEngineTypesQueryResult(PageResponse<SearchEngineTypeDto> result);
    internal class GetSearchEngineTypesQueryHandler(IRepository<SearchEngineType> repository, IMapper mapper)
        : IQueryHandler<SearchEngineTypesQuery, SearchEngineTypesQueryResult>
    {
        private readonly IRepository<SearchEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<SearchEngineTypesQueryResult> Handle(SearchEngineTypesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<SearchEngineTypeDto>>(entitiy);
            return new SearchEngineTypesQueryResult(dto);
        }
    }
}
