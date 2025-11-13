using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ChunkingManagement.ApplicationChunkingStrategyHandlers
{
    public record ApplicationChunkingStrategyQuery(long Id):IQuery<ApplicationChunkingStrategyQueryResult>;
    public record ApplicationChunkingStrategyQueryResult(ApplicationChunkingStrategyDto result);
    internal class GetApplicationChunkingStrategyQueryHandler(IRepository<ApplicationChunkingStrategy> repository, IMapper mapper)
        : IQueryHandler<ApplicationChunkingStrategyQuery, ApplicationChunkingStrategyQueryResult>
    {
        private readonly IRepository<ApplicationChunkingStrategy> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationChunkingStrategyQueryResult> Handle(ApplicationChunkingStrategyQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ApplicationChunkingStrategyDto>(entitiy);
            return new ApplicationChunkingStrategyQueryResult(dto);
        }
    }
}
