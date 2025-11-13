using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.LlmManagement.LlmProviderHandlers
{
    public record LlmProvidersQuery(PageRequest PageRequest):IQuery<LlmProvidersQueryResult>;
    public record LlmProvidersQueryResult(PageResponse<LlmProviderDto> result);
    internal class GetLlmProvidersQueryHandler(IRepository<LlmProvider> repository, IMapper mapper)
        : IQueryHandler<LlmProvidersQuery, LlmProvidersQueryResult>
    {
        private readonly IRepository<LlmProvider> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<LlmProvidersQueryResult> Handle(LlmProvidersQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<LlmProviderDto>>(entitiy);
            return new LlmProvidersQueryResult(dto);
        }
    }
}
