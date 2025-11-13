using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.FileManagement.IngestionStatusTypeHandlers
{
    public record IngestionStatusTypeQuery(IngestionStatusTypes Id):IQuery<IngestionStatusTypeQueryResult>;
    public record IngestionStatusTypeQueryResult(IngestionStatusTypeDto result);
    internal class GetIngestionStatusTypeQueryHandler(IRepository<IngestionStatusType> repository, IMapper mapper)
        : IQueryHandler<IngestionStatusTypeQuery, IngestionStatusTypeQueryResult>
    {
        private readonly IRepository<IngestionStatusType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<IngestionStatusTypeQueryResult> Handle(IngestionStatusTypeQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<IngestionStatusTypeDto>(entitiy);
            return new IngestionStatusTypeQueryResult(dto);
        }
    }
}
