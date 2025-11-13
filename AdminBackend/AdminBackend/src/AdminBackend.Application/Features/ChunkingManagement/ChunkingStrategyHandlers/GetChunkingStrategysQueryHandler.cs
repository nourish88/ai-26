using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ChunkingManagement.ChunkingStrategyHandlers
{
    public record ChunkingStrategysQuery(PageRequest PageRequest):IQuery<ChunkingStrategysQueryResult>;
    public record ChunkingStrategysQueryResult(PageResponse<ChunkingStrategyDto> result);
    internal class GetChunkingStrategysQueryHandler(IRepository<ChunkingStrategy> repository, IMapper mapper)
        : IQueryHandler<ChunkingStrategysQuery, ChunkingStrategysQueryResult>
    {
        private readonly IRepository<ChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ChunkingStrategysQueryResult> Handle(ChunkingStrategysQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ChunkingStrategyDto>>(entitiy);
            return new ChunkingStrategysQueryResult(dto);
        }
    }
}
