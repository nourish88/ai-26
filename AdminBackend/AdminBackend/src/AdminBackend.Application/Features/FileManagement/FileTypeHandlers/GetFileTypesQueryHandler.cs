using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.FileManagement.FileTypeHandlers
{
    public record FileTypesQuery(PageRequest PageRequest):IQuery<FileTypesQueryResult>;
    public record FileTypesQueryResult(PageResponse<FileTypeDto> result);
    internal class GetFileTypesQueryHandler(IRepository<FileType> repository, IMapper mapper)
        : IQueryHandler<FileTypesQuery, FileTypesQueryResult>
    {
        private readonly IRepository<FileType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<FileTypesQueryResult> Handle(FileTypesQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<FileTypeDto>>(entitiy);
            return new FileTypesQueryResult(dto);
        }
    }
}
