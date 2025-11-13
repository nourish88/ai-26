using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.FileManagement.FileTypeHandlers
{
    public record FileTypeQuery(FileTypes Id):IQuery<FileTypeQueryResult>;
    public record FileTypeQueryResult(FileTypeDto result);
    internal class GetFileTypeQueryHandler(IRepository<FileType> repository, IMapper mapper)
        : IQueryHandler<FileTypeQuery, FileTypeQueryResult>
    {
        private readonly IRepository<FileType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<FileTypeQueryResult> Handle(FileTypeQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<FileTypeDto>(entitiy);
            return new FileTypeQueryResult(dto);
        }
    }
}
