using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationTypeHandlers
{
    public record ApplicationTypeQuery(ApplicationTypes Id):IQuery<ApplicationTypeQueryResult>;
    public record ApplicationTypeQueryResult(ApplicationTypeDto result);
    internal class GetApplicationTypeQueryHandler(IRepository<ApplicationType> repository, IMapper mapper)
        : IQueryHandler<ApplicationTypeQuery, ApplicationTypeQueryResult>
    {
        private readonly IRepository<ApplicationType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationTypeQueryResult> Handle(ApplicationTypeQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ApplicationTypeDto>(entitiy);
            return new ApplicationTypeQueryResult(dto);
        }
    }
}
