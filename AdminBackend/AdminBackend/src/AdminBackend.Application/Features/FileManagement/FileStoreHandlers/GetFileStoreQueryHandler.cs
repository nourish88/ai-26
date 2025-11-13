using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.FileManagement.FileStoreHandlers
{
    public record FileStoreQuery(long Id):IQuery<FileStoreQueryResult>;
    public record FileStoreQueryResult(FileStoreDto result);
    internal class GetFileStoreQueryHandler(IRepository<FileStore> repository, IMapper mapper)
        : IQueryHandler<FileStoreQuery, FileStoreQueryResult>
    {
        private readonly IRepository<FileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<FileStoreQueryResult> Handle(FileStoreQuery request, CancellationToken cancellationToken)
        {
            var entity = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<FileStoreDto>(entity);
            return new FileStoreQueryResult(dto);
        }
    }
}
