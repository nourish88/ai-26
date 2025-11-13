using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.SearchEngineManagement.ApplicationSearchEngineHandlers
{
    public record ApplicationSearchEngineQuery(long Id):IQuery<ApplicationSearchEngineQueryResult>;
    public record ApplicationSearchEngineQueryResult(ApplicationSearchEngineDto result);
    internal class GetApplicationSearchEngineQueryHandler(IRepository<ApplicationSearchEngine> repository, IMapper mapper)
        : IQueryHandler<ApplicationSearchEngineQuery, ApplicationSearchEngineQueryResult>
    {
        private readonly IRepository<ApplicationSearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationSearchEngineQueryResult> Handle(ApplicationSearchEngineQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ApplicationSearchEngineDto>(entitiy);
            return new ApplicationSearchEngineQueryResult(dto);
        }
    }
}
