using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.LlmManagement.LlmProviderHandlers
{

    public record DeleteLlmProviderCommand(long Id):ICommand<DeleteLlmProviderCommandResult>;
    public record DeleteLlmProviderCommandResult(bool result);    

    internal class DeleteLlmProviderCommandHandler(IRepository<LlmProvider> repository, IMapper mapper) 
        : ICommandHandler<DeleteLlmProviderCommand, DeleteLlmProviderCommandResult>
    {
        private readonly IRepository<LlmProvider> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteLlmProviderCommandResult> Handle(DeleteLlmProviderCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteLlmProviderCommandResult(true);
        }
    }
}
