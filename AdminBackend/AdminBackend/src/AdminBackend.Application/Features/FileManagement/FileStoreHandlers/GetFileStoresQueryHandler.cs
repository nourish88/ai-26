using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Juga.Data.Paging;

namespace AdminBackend.Application.Features.FileManagement.FileStoreHandlers
{
    public record FileStoresQuery(PageRequest PageRequest):IQuery<FileStoresQueryResult>;
    public record FileStoresQueryResult(PageResponse<FileStoreDto> result);
    internal class GetFileStoresQueryHandler(IRepository<FileStore> repository, IMapper mapper)
        : IQueryHandler<FileStoresQuery, FileStoresQueryResult>
    {
        private readonly IRepository<FileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<FileStoresQueryResult> Handle(FileStoresQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.GetPaginatedListAsync(
                index: request.PageRequest.PageIndex,
                size:request.PageRequest.PageSize,
                cancellationToken:cancellationToken);
            var dto = mapper.Map<PageResponse<FileStoreDto>>(entitiy);
            return new FileStoresQueryResult(dto);
        }
    }
}
