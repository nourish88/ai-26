using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationLlmHandlers
{
    public record ApplicationLlmQuery(long Id):IQuery<ApplicationLlmQueryResult>;
    public record ApplicationLlmQueryResult(ApplicationLlmDto result);
    internal class GetApplicationLlmQueryHandler(IRepository<ApplicationLlm> repository, IMapper mapper)
        : IQueryHandler<ApplicationLlmQuery, ApplicationLlmQueryResult>
    {
        private readonly IRepository<ApplicationLlm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationLlmQueryResult> Handle(ApplicationLlmQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ApplicationLlmDto>(entitiy);
            return new ApplicationLlmQueryResult(dto);
        }
    }
}
