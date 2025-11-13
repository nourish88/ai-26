using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.LlmManagement.LlmProviderHandlers
{
    public record LlmProviderQuery(long Id):IQuery<LlmProviderQueryResult>;
    public record LlmProviderQueryResult(LlmProviderDto result);
    internal class GetLlmProviderQueryHandler(IRepository<LlmProvider> repository, IMapper mapper)
        : IQueryHandler<LlmProviderQuery, LlmProviderQueryResult>
    {
        private readonly IRepository<LlmProvider> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<LlmProviderQueryResult> Handle(LlmProviderQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<LlmProviderDto>(entitiy);
            return new LlmProviderQueryResult(dto);
        }
    }
}
