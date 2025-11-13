using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineHandlers
{
    public record SearchEngineQuery(long Id):IQuery<SearchEngineQueryResult>;
    public record SearchEngineQueryResult(SearchEngineDto result);
    internal class GetSearchEngineQueryHandler(IRepository<SearchEngine> repository, IMapper mapper)
        : IQueryHandler<SearchEngineQuery, SearchEngineQueryResult>
    {
        private readonly IRepository<SearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<SearchEngineQueryResult> Handle(SearchEngineQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<SearchEngineDto>(entitiy);
            return new SearchEngineQueryResult(dto);
        }
    }
}
