using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationLlmHandlers
{

    public record DeleteApplicationLlmCommand(long Id):ICommand<DeleteApplicationLlmCommandResult>;
    public record DeleteApplicationLlmCommandResult(bool result);    

    internal class DeleteApplicationLlmCommandHandler(IRepository<ApplicationLlm> repository, IMapper mapper) 
        : ICommandHandler<DeleteApplicationLlmCommand, DeleteApplicationLlmCommandResult>
    {
        private readonly IRepository<ApplicationLlm> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteApplicationLlmCommandResult> Handle(DeleteApplicationLlmCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteApplicationLlmCommandResult(true);
        }
    }
}
