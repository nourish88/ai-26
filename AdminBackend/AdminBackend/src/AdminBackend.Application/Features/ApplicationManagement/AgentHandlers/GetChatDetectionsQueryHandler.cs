using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ApplicationManagement.AgentHandlers
{
    public record ChatDetectionsQuery(PageRequest PageRequest):IQuery<ChatDetectionsQueryResult>;
    public record ChatDetectionsQueryResult(PageResponse<ChatDetectionDto> result);
    internal class GetChatDetectionsQueryHandler(IRepository<ChatDetection> repository, IMapper mapper)
        : IQueryHandler<ChatDetectionsQuery, ChatDetectionsQueryResult>
    {
        private readonly IRepository<ChatDetection> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ChatDetectionsQueryResult> Handle(ChatDetectionsQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ChatDetectionDto>>(entitiy);
            return new ChatDetectionsQueryResult(dto);
        }
    }
}
