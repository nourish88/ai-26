using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.FileManagement.ApplicationFileStoreHandlers
{
    public record ApplicationFileStoreQuery(long Id):IQuery<ApplicationFileStoreQueryResult>;
    public record ApplicationFileStoreQueryResult(ApplicationFileStoreDto result);
    internal class GetApplicationFileStoreQueryHandler(IRepository<ApplicationFileStore> repository, IMapper mapper)
        : IQueryHandler<ApplicationFileStoreQuery, ApplicationFileStoreQueryResult>
    {
        private readonly IRepository<ApplicationFileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationFileStoreQueryResult> Handle(ApplicationFileStoreQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ApplicationFileStoreDto>(entitiy);
            return new ApplicationFileStoreQueryResult(dto);
        }
    }
}
