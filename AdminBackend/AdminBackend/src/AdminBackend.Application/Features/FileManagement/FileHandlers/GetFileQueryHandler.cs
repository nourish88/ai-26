using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers
{
    public record FileQuery(long Id):IQuery<FileQueryResult>;
    public record FileQueryResult(FileDto result);
    internal class GetFileQueryHandler(IRepository<Domain.Entities.File> repository, IMapper mapper)
        : IQueryHandler<FileQuery, FileQueryResult>
    {
        private readonly IRepository<Domain.Entities.File> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<FileQueryResult> Handle(FileQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<FileDto>(entitiy);
            return new FileQueryResult(dto);
        }
    }
}
