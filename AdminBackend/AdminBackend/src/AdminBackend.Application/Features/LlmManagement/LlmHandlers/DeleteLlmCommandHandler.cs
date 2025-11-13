using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.LlmManagement.LlmHandlers
{

    public record DeleteLlmCommand(long Id):ICommand<DeleteLlmCommandResult>;
    public record DeleteLlmCommandResult(bool result);    

    internal class DeleteLlmCommandHandler(IRepository<Llm> repository, IMapper mapper) 
        : ICommandHandler<DeleteLlmCommand, DeleteLlmCommandResult>
    {
        private readonly IRepository<Llm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteLlmCommandResult> Handle(DeleteLlmCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteLlmCommandResult(true);
        }
    }
}
