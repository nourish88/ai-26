using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.LlmManagement.LlmHandlers
{
    public record LlmsQuery(PageRequest PageRequest):IQuery<LlmsQueryResult>;
    public record LlmsQueryResult(PageResponse<LlmDto> result);
    internal class GetLlmsQueryHandler(IRepository<Llm> repository, IMapper mapper)
        : IQueryHandler<LlmsQuery, LlmsQueryResult>
    {
        private readonly IRepository<Llm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<LlmsQueryResult> Handle(LlmsQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<LlmDto>>(entitiy);
            return new LlmsQueryResult(dto);
        }
    }
}
