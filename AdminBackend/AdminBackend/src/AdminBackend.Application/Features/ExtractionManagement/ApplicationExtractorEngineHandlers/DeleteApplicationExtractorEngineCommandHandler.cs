using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ExtractionManagement.ApplicationExtractorEngineHandlers
{

    public record DeleteApplicationExtractorEngineCommand(long Id):ICommand<DeleteApplicationExtractorEngineCommandResult>;
    public record DeleteApplicationExtractorEngineCommandResult(bool result);    

    internal class DeleteApplicationExtractorEngineCommandHandler(IRepository<ApplicationExtractorEngine> repository, IMapper mapper) 
        : ICommandHandler<DeleteApplicationExtractorEngineCommand, DeleteApplicationExtractorEngineCommandResult>
    {
        private readonly IRepository<ApplicationExtractorEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteApplicationExtractorEngineCommandResult> Handle(DeleteApplicationExtractorEngineCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteApplicationExtractorEngineCommandResult(true);
        }
    }
}
