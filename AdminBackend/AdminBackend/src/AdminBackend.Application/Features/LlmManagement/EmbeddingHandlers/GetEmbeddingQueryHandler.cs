using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.LlmManagement.EmbeddingHandlers
{
    public record EmbeddingQuery(long Id):IQuery<EmbeddingQueryResult>;
    public record EmbeddingQueryResult(EmbeddingDto result);
    internal class GetEmbeddingQueryHandler(IRepository<Embedding> repository, IMapper mapper)
        : IQueryHandler<EmbeddingQuery, EmbeddingQueryResult>
    {
        private readonly IRepository<Embedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<EmbeddingQueryResult> Handle(EmbeddingQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<EmbeddingDto>(entitiy);
            return new EmbeddingQueryResult(dto);
        }
    }
}
