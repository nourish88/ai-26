using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationTypeHandlers
{
    public record ApplicationTypesQuery(PageRequest PageRequest):IQuery<ApplicationTypesQueryResult>;
    public record ApplicationTypesQueryResult(PageResponse<ApplicationTypeDto> result);
    internal class GetApplicationTypesQueryHandler(IRepository<ApplicationType> repository, IMapper mapper)
        : IQueryHandler<ApplicationTypesQuery, ApplicationTypesQueryResult>
    {
        private readonly IRepository<ApplicationType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationTypesQueryResult> Handle(ApplicationTypesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ApplicationTypeDto>>(entitiy);
            return new ApplicationTypesQueryResult(dto);
        }
    }
}
