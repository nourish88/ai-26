using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.SearchEngineManagement.SearchEngineTypeHandlers
{

    public record DeleteSearchEngineTypeCommand(long Id):ICommand<DeleteSearchEngineTypeCommandResult>;
    public record DeleteSearchEngineTypeCommandResult(bool result);    

    internal class DeleteSearchEngineTypeCommandHandler(IRepository<SearchEngineType> repository, IMapper mapper) 
        : ICommandHandler<DeleteSearchEngineTypeCommand, DeleteSearchEngineTypeCommandResult>
    {
        private readonly IRepository<SearchEngineType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteSearchEngineTypeCommandResult> Handle(DeleteSearchEngineTypeCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteSearchEngineTypeCommandResult(true);
        }
    }
}
