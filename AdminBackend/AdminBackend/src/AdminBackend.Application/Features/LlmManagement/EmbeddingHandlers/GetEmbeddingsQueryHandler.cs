using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.LlmManagement.EmbeddingHandlers
{
    public record EmbeddingsQuery(PageRequest PageRequest):IQuery<EmbeddingsQueryResult>;
    public record EmbeddingsQueryResult(PageResponse<EmbeddingDto> result);
    internal class GetEmbeddingsQueryHandler(IRepository<Embedding> repository, IMapper mapper)
        : IQueryHandler<EmbeddingsQuery, EmbeddingsQueryResult>
    {
        private readonly IRepository<Embedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<EmbeddingsQueryResult> Handle(EmbeddingsQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<EmbeddingDto>>(entitiy);
            return new EmbeddingsQueryResult(dto);
        }
    }
}
