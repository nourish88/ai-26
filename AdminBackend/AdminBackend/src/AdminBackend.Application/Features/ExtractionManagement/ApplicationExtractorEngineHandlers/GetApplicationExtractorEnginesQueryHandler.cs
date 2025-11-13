using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ExtractionManagement.ApplicationExtractorEngineHandlers
{
    public record ApplicationExtractorEnginesQuery(PageRequest PageRequest):IQuery<ApplicationExtractorEnginesQueryResult>;
    public record ApplicationExtractorEnginesQueryResult(PageResponse<ApplicationExtractorEngineDto> result);
    internal class GetApplicationExtractorEnginesQueryHandler(IRepository<ApplicationExtractorEngine> repository, IMapper mapper)
        : IQueryHandler<ApplicationExtractorEnginesQuery, ApplicationExtractorEnginesQueryResult>
    {
        private readonly IRepository<ApplicationExtractorEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationExtractorEnginesQueryResult> Handle(ApplicationExtractorEnginesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ApplicationExtractorEngineDto>>(entitiy);
            return new ApplicationExtractorEnginesQueryResult(dto);
        }
    }
}
