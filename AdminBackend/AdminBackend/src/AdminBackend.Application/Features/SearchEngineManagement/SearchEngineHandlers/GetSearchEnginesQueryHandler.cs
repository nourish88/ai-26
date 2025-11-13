using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineHandlers
{
    public record SearchEnginesQuery(PageRequest PageRequest):IQuery<SearchEnginesQueryResult>;
    public record SearchEnginesQueryResult(PageResponse<SearchEngineDto> result);
    internal class GetSearchEnginesQueryHandler(IRepository<SearchEngine> repository, IMapper mapper)
        : IQueryHandler<SearchEnginesQuery, SearchEnginesQueryResult>
    {
        private readonly IRepository<SearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<SearchEnginesQueryResult> Handle(SearchEnginesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<SearchEngineDto>>(entitiy);
            return new SearchEnginesQueryResult(dto);
        }
    }
}
