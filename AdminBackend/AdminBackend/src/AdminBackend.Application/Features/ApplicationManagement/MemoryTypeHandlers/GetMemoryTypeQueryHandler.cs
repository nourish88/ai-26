using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ApplicationManagement.MemoryTypeHandlers
{
    public record MemoryTypeQuery(MemoryTypes Id):IQuery<MemoryTypeQueryResult>;
    public record MemoryTypeQueryResult(MemoryTypeDto result);
    internal class GetMemoryTypeQueryHandler(IRepository<MemoryType> repository, IMapper mapper)
        : IQueryHandler<MemoryTypeQuery, MemoryTypeQueryResult>
    {
        private readonly IRepository<MemoryType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<MemoryTypeQueryResult> Handle(MemoryTypeQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<MemoryTypeDto>(entitiy);
            return new MemoryTypeQueryResult(dto);
        }
    }
}
