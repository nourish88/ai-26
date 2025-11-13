using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationMcpServerHandlers
{

    public record DeleteApplicationMcpServerCommand(long Id):ICommand<DeleteApplicationMcpServerCommandResult>;
    public record DeleteApplicationMcpServerCommandResult(bool result);    

    internal class DeleteApplicationMcpServerCommandHandler(IRepository<ApplicationMcpServer> repository, IMapper mapper) 
        : ICommandHandler<DeleteApplicationMcpServerCommand, DeleteApplicationMcpServerCommandResult>
    {
        private readonly IRepository<ApplicationMcpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteApplicationMcpServerCommandResult> Handle(DeleteApplicationMcpServerCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteApplicationMcpServerCommandResult(true);
        }
    }
}
