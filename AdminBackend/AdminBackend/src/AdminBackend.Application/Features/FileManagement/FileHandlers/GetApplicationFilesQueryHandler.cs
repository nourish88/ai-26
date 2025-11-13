using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers
{
    public class ApplicationFilesPagedRequest : PageRequest
    {
        public int ApplicationId { get; set; }
    }
    public record ApplicationFilesQuery(ApplicationFilesPagedRequest PageRequest):IQuery<ApplicationFilesQueryResult>;
    public record ApplicationFilesQueryResult(PageResponse<FileDto> result);
    internal class ApplicationGetFilesQueryHandler(IRepository<Domain.Entities.File> repository, IMapper mapper)
        : IQueryHandler<ApplicationFilesQuery, ApplicationFilesQueryResult>
    {
        private readonly IRepository<Domain.Entities.File> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationFilesQueryResult> Handle(ApplicationFilesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                predicate:p=>p.UploadApplicationId == request.PageRequest.ApplicationId,
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<FileDto>>(entitiy);
            return new ApplicationFilesQueryResult(dto);
        }
    }
}
