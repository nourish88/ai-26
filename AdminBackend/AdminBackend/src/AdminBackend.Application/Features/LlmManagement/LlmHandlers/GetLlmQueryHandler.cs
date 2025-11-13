using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.LlmManagement.LlmHandlers
{
    public record LlmQuery(long Id):IQuery<LlmQueryResult>;
    public record LlmQueryResult(LlmDto result);
    internal class GetLlmQueryHandler(IRepository<Llm> repository, IMapper mapper)
        : IQueryHandler<LlmQuery, LlmQueryResult>
    {
        private readonly IRepository<Llm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<LlmQueryResult> Handle(LlmQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<LlmDto>(entitiy);
            return new LlmQueryResult(dto);
        }
    }
}
