using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.FileManagement.IngestionStatusTypeHandlers
{
    public record IngestionStatusTypesQuery(PageRequest PageRequest):IQuery<IngestionStatusTypesQueryResult>;
    public record IngestionStatusTypesQueryResult(PageResponse<IngestionStatusTypeDto> result);
    internal class GetIngestionStatusTypesQueryHandler(IRepository<IngestionStatusType> repository, IMapper mapper)
        : IQueryHandler<IngestionStatusTypesQuery, IngestionStatusTypesQueryResult>
    {
        private readonly IRepository<IngestionStatusType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<IngestionStatusTypesQueryResult> Handle(IngestionStatusTypesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<IngestionStatusTypeDto>>(entitiy);
            return new IngestionStatusTypesQueryResult(dto);
        }
    }
}
