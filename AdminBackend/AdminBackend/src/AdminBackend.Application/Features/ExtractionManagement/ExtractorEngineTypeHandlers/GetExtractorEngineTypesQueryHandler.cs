using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ExtractionManagement.ExtractorEngineTypeHandlers
{
    public record ExtractorEngineTypesQuery(PageRequest PageRequest):IQuery<ExtractorEngineTypesQueryResult>;
    public record ExtractorEngineTypesQueryResult(PageResponse<ExtractorEngineTypeDto> result);
    internal class GetExtractorEngineTypesQueryHandler(IRepository<ExtractorEngineType> repository, IMapper mapper)
        : IQueryHandler<ExtractorEngineTypesQuery, ExtractorEngineTypesQueryResult>
    {
        private readonly IRepository<ExtractorEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ExtractorEngineTypesQueryResult> Handle(ExtractorEngineTypesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ExtractorEngineTypeDto>>(entitiy);
            return new ExtractorEngineTypesQueryResult(dto);
        }
    }
}
