using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.FileManagement.ApplicationFileStoreHandlers
{
    public record ApplicationFileStoresQuery(PageRequest PageRequest):IQuery<ApplicationFileStoresQueryResult>;
    public record ApplicationFileStoresQueryResult(PageResponse<ApplicationFileStoreDto> result);
    internal class GetApplicationFileStoresQueryHandler(IRepository<ApplicationFileStore> repository, IMapper mapper)
        : IQueryHandler<ApplicationFileStoresQuery, ApplicationFileStoresQueryResult>
    {
        private readonly IRepository<ApplicationFileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationFileStoresQueryResult> Handle(ApplicationFileStoresQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ApplicationFileStoreDto>>(entitiy);
            return new ApplicationFileStoresQueryResult(dto);
        }
    }
}
