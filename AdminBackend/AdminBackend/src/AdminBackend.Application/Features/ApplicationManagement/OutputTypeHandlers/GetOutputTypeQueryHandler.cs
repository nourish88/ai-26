using AdminBackend.Application.Dtos;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AdminBackend.Application.Features.ApplicationManagement.OutputTypeHandlers
{
    public record OutputTypeQuery(OutputTypes Id):IQuery<OutputTypeQueryResult>;
    public record OutputTypeQueryResult(OutputTypeDto result);
    internal class GetOutputTypeQueryHandler(IRepository<OutputType> repository, IMapper mapper)
        : IQueryHandler<OutputTypeQuery, OutputTypeQueryResult>
    {
        private readonly IRepository<OutputType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<OutputTypeQueryResult> Handle(OutputTypeQuery request, CancellationToken cancellationToken)
        {
            var entitiy = await repository.Where(x=>x.Id == request.Id).FirstOrDefaultAsync(cancellationToken);
            var dto = mapper.Map<OutputTypeDto>(entitiy);
            return new OutputTypeQueryResult(dto);
        }
    }
}
