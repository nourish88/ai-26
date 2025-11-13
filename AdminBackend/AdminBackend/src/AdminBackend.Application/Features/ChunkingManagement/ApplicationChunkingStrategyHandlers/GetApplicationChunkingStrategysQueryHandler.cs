using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ChunkingManagement.ApplicationChunkingStrategyHandlers
{
    public record ApplicationChunkingStrategysQuery(PageRequest PageRequest):IQuery<ApplicationChunkingStrategysQueryResult>;
    public record ApplicationChunkingStrategysQueryResult(PageResponse<ApplicationChunkingStrategyDto> result);
    internal class GetApplicationChunkingStrategysQueryHandler(IRepository<ApplicationChunkingStrategy> repository, IMapper mapper)
        : IQueryHandler<ApplicationChunkingStrategysQuery, ApplicationChunkingStrategysQueryResult>
    {
        private readonly IRepository<ApplicationChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationChunkingStrategysQueryResult> Handle(ApplicationChunkingStrategysQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ApplicationChunkingStrategyDto>>(entitiy);
            return new ApplicationChunkingStrategysQueryResult(dto);
        }
    }
}
