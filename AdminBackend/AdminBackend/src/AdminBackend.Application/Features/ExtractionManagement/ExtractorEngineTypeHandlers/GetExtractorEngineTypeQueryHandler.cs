using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ExtractionManagement.ExtractorEngineTypeHandlers
{
    public record ExtractorEngineTypeQuery(long Id):IQuery<ExtractorEngineTypeQueryResult>;
    public record ExtractorEngineTypeQueryResult(ExtractorEngineTypeDto result);
    internal class GetExtractorEngineTypeQueryHandler(IRepository<ExtractorEngineType> repository, IMapper mapper)
        : IQueryHandler<ExtractorEngineTypeQuery, ExtractorEngineTypeQueryResult>
    {
        private readonly IRepository<ExtractorEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ExtractorEngineTypeQueryResult> Handle(ExtractorEngineTypeQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ExtractorEngineTypeDto>(entitiy);
            return new ExtractorEngineTypeQueryResult(dto);
        }
    }
}
