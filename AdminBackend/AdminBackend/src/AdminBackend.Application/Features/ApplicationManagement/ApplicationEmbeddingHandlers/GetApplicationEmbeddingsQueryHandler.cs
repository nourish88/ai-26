using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationEmbeddingHandlers
{
    public record ApplicationEmbeddingsQuery(PageRequest PageRequest):IQuery<ApplicationEmbeddingsQueryResult>;
    public record ApplicationEmbeddingsQueryResult(PageResponse<ApplicationEmbeddingDto> result);
    internal class GetApplicationEmbeddingsQueryHandler(IRepository<ApplicationEmbedding> repository, IMapper mapper)
        : IQueryHandler<ApplicationEmbeddingsQuery, ApplicationEmbeddingsQueryResult>
    {
        private readonly IRepository<ApplicationEmbedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationEmbeddingsQueryResult> Handle(ApplicationEmbeddingsQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ApplicationEmbeddingDto>>(entitiy);
            return new ApplicationEmbeddingsQueryResult(dto);
        }
    }
}
