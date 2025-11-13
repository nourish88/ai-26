using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationMcpServerHandlers
{
    public record ApplicationMcpServerQuery(long Id):IQuery<ApplicationMcpServerQueryResult>;
    public record ApplicationMcpServerQueryResult(ApplicationMcpServerDto result);
    internal class GetApplicationMcpServerQueryHandler(IRepository<ApplicationMcpServer> repository, IMapper mapper)
        : IQueryHandler<ApplicationMcpServerQuery, ApplicationMcpServerQueryResult>
    {
        private readonly IRepository<ApplicationMcpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<ApplicationMcpServerQueryResult> Handle(ApplicationMcpServerQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<ApplicationMcpServerDto>(entitiy);
            return new ApplicationMcpServerQueryResult(dto);
        }
    }
}
