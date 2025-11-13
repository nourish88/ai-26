using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.LlmManagement.EmbeddingHandlers
{

    public record DeleteEmbeddingCommand(long Id):ICommand<DeleteEmbeddingCommandResult>;
    public record DeleteEmbeddingCommandResult(bool result);    

    internal class DeleteEmbeddingCommandHandler(IRepository<Embedding> repository, IMapper mapper) 
        : ICommandHandler<DeleteEmbeddingCommand, DeleteEmbeddingCommandResult>
    {
        private readonly IRepository<Embedding> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteEmbeddingCommandResult> Handle(DeleteEmbeddingCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteEmbeddingCommandResult(true);
        }
    }
}
