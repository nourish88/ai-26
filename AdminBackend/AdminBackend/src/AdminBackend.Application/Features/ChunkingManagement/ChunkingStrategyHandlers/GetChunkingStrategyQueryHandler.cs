using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ChunkingManagement.ChunkingStrategyHandlers
{
    public record ChunkingStrategyQuery(long Id):IQuery<ChunkingStrategyQueryResult>;
    public record ChunkingStrategyQueryResult(ChunkingStrategyDto result);
    internal class GetChunkingStrategyQueryHandler(IRepository<ChunkingStrategy> repository, IMapper mapper)
        : IQueryHandler<ChunkingStrategyQuery, ChunkingStrategyQueryResult>
    {
        private readonly IRepository<ChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ChunkingStrategyQueryResult> Handle(ChunkingStrategyQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ChunkingStrategyDto>(entitiy);
            return new ChunkingStrategyQueryResult(dto);
        }
    }
}
