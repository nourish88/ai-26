using AdminBackend.Application.Dtos;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationHandlers
{
    public record ApplicationsQuery(PageRequest PageRequest):IQuery<ApplicationsQueryResult>;
    public record ApplicationsQueryResult(PageResponse<ApplicationDto> result);
    internal class GetApplicationsQueryHandler(IRepository<Domain.Entities.Application> repository, IMapper mapper)
        : IQueryHandler<ApplicationsQuery, ApplicationsQueryResult>
    {
        private readonly IRepository<Domain.Entities.Application> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationsQueryResult> Handle(ApplicationsQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ApplicationDto>>(entitiy);
            return new ApplicationsQueryResult(dto);
        }
    }
}
