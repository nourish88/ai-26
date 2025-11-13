using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationEmbeddingHandlers
{
    public record ApplicationEmbeddingQuery(long Id):IQuery<ApplicationEmbeddingQueryResult>;
    public record ApplicationEmbeddingQueryResult(ApplicationEmbeddingDto result);
    internal class GetApplicationEmbeddingQueryHandler(IRepository<ApplicationEmbedding> repository, IMapper mapper)
        : IQueryHandler<ApplicationEmbeddingQuery, ApplicationEmbeddingQueryResult>
    {
        private readonly IRepository<ApplicationEmbedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationEmbeddingQueryResult> Handle(ApplicationEmbeddingQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ApplicationEmbeddingDto>(entitiy);
            return new ApplicationEmbeddingQueryResult(dto);
        }
    }
}
