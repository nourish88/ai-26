using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineTypeHandlers
{
    public record SearchEngineTypeQuery(long Id):IQuery<SearchEngineTypeQueryResult>;
    public record SearchEngineTypeQueryResult(SearchEngineTypeDto result);
    internal class GetSearchEngineTypeQueryHandler(IRepository<SearchEngineType> repository, IMapper mapper)
        : IQueryHandler<SearchEngineTypeQuery, SearchEngineTypeQueryResult>
    {
        private readonly IRepository<SearchEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<SearchEngineTypeQueryResult> Handle(SearchEngineTypeQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<SearchEngineTypeDto>(entitiy);
            return new SearchEngineTypeQueryResult(dto);
        }
    }
}
