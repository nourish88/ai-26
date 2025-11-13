using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.ApplicationManagement.OutputTypeHandlers
{
    public record OutputTypesQuery(PageRequest PageRequest):IQuery<OutputTypesQueryResult>;
    public record OutputTypesQueryResult(PageResponse<OutputTypeDto> result);
    internal class GetOutputTypesQueryHandler(IRepository<OutputType> repository, IMapper mapper)
        : IQueryHandler<OutputTypesQuery, OutputTypesQueryResult>
    {
        private readonly IRepository<OutputType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<OutputTypesQueryResult> Handle(OutputTypesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<OutputTypeDto>>(entitiy);
            return new OutputTypesQueryResult(dto);
        }
    }
}
