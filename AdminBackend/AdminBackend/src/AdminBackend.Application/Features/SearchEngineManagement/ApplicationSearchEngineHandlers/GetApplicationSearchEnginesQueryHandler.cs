using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.SearchEngineManagement.ApplicationSearchEngineHandlers
{
    public record ApplicationSearchEnginesQuery(PageRequest PageRequest):IQuery<ApplicationSearchEnginesQueryResult>;
    public record ApplicationSearchEnginesQueryResult(PageResponse<ApplicationSearchEngineDto> result);
    internal class GetApplicationSearchEnginesQueryHandler(IRepository<ApplicationSearchEngine> repository, IMapper mapper)
        : IQueryHandler<ApplicationSearchEnginesQuery, ApplicationSearchEnginesQueryResult>
    {
        private readonly IRepository<ApplicationSearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationSearchEnginesQueryResult> Handle(ApplicationSearchEnginesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ApplicationSearchEngineDto>>(entitiy);
            return new ApplicationSearchEnginesQueryResult(dto);
        }
    }
}
