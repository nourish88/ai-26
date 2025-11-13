using AdminBackend.Application.Dtos;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationHandlers
{
    public record ApplicationQuery(long Id):IQuery<ApplicationQueryResult>;
    public record ApplicationQueryResult(ApplicationDto result);
    internal class GetApplicationQueryHandler(IRepository<Domain.Entities.Application> repository, IMapper mapper)
        : IQueryHandler<ApplicationQuery, ApplicationQueryResult>
    {
        private readonly IRepository<Domain.Entities.Application> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationQueryResult> Handle(ApplicationQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ApplicationDto>(entitiy);
            return new ApplicationQueryResult(dto);
        }
    }
}
