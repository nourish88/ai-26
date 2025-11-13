using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationEmbeddingHandlers
{

    public record DeleteApplicationEmbeddingCommand(long Id):ICommand<DeleteApplicationEmbeddingCommandResult>;
    public record DeleteApplicationEmbeddingCommandResult(bool result);    

    internal class DeleteApplicationEmbeddingCommandHandler(IRepository<ApplicationEmbedding> repository, IMapper mapper) 
        : ICommandHandler<DeleteApplicationEmbeddingCommand, DeleteApplicationEmbeddingCommandResult>
    {
        private readonly IRepository<ApplicationEmbedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteApplicationEmbeddingCommandResult> Handle(DeleteApplicationEmbeddingCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteApplicationEmbeddingCommandResult(true);
        }
    }
}
