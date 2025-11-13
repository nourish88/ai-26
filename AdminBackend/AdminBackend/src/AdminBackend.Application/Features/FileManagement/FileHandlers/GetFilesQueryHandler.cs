using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers
{
    public record FilesQuery(PageRequest PageRequest):IQuery<FilesQueryResult>;
    public record FilesQueryResult(PageResponse<FileDto> result);
    internal class GetFilesQueryHandler(IRepository<Domain.Entities.File> repository, IMapper mapper)
        : IQueryHandler<FilesQuery, FilesQueryResult>
    {
        private readonly IRepository<Domain.Entities.File> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<FilesQueryResult> Handle(FilesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<FileDto>>(entitiy);
            return new FilesQueryResult(dto);
        }
    }
}
