using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationLlmHandlers
{
    public record ApplicationLlmsQuery(PageRequest PageRequest):IQuery<ApplicationLlmsQueryResult>;
    public record ApplicationLlmsQueryResult(PageResponse<ApplicationLlmDto> result);
    internal class GetApplicationLlmsQueryHandler(IRepository<ApplicationLlm> repository, IMapper mapper)
        : IQueryHandler<ApplicationLlmsQuery, ApplicationLlmsQueryResult>
    {
        private readonly IRepository<ApplicationLlm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationLlmsQueryResult> Handle(ApplicationLlmsQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<ApplicationLlmDto>>(entitiy);
            return new ApplicationLlmsQueryResult(dto);
        }
    }
}
