using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ExtractionManagement.ApplicationExtractorEngineHandlers
{
    public record ApplicationExtractorEngineQuery(long Id):IQuery<ApplicationExtractorEngineQueryResult>;
    public record ApplicationExtractorEngineQueryResult(ApplicationExtractorEngineDto result);
    internal class GetApplicationExtractorEngineQueryHandler(IRepository<ApplicationExtractorEngine> repository, IMapper mapper)
        : IQueryHandler<ApplicationExtractorEngineQuery, ApplicationExtractorEngineQueryResult>
    {
        private readonly IRepository<ApplicationExtractorEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationExtractorEngineQueryResult> Handle(ApplicationExtractorEngineQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ApplicationExtractorEngineDto>(entitiy);
            return new ApplicationExtractorEngineQueryResult(dto);
        }
    }
}
