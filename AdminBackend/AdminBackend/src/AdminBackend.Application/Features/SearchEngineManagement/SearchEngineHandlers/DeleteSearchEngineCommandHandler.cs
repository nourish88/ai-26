using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineHandlers
{

    public record DeleteSearchEngineCommand(long Id):ICommand<DeleteSearchEngineCommandResult>;
    public record DeleteSearchEngineCommandResult(bool result);    

    internal class DeleteSearchEngineCommandHandler(IRepository<SearchEngine> repository, IMapper mapper) 
        : ICommandHandler<DeleteSearchEngineCommand, DeleteSearchEngineCommandResult>
    {
        private readonly IRepository<SearchEngine> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteSearchEngineCommandResult> Handle(DeleteSearchEngineCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteSearchEngineCommandResult(true);
        }
    }
}
