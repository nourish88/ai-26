using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.SearchEngineManagement.ApplicationSearchEngineHandlers
{

    public record DeleteApplicationSearchEngineCommand(long Id):ICommand<DeleteApplicationSearchEngineCommandResult>;
    public record DeleteApplicationSearchEngineCommandResult(bool result);    

    internal class DeleteApplicationSearchEngineCommandHandler(IRepository<ApplicationSearchEngine> repository, IMapper mapper) 
        : ICommandHandler<DeleteApplicationSearchEngineCommand, DeleteApplicationSearchEngineCommandResult>
    {
        private readonly IRepository<ApplicationSearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteApplicationSearchEngineCommandResult> Handle(DeleteApplicationSearchEngineCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteApplicationSearchEngineCommandResult(true);
        }
    }
}
