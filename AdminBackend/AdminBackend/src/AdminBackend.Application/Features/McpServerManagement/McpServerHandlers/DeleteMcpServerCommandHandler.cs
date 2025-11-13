using AdminBackend.Domain.Entities;
using AutoMapper;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.McpServerManagement.McpServerHandlers
{

    public record DeleteMcpServerCommand(long Id):ICommand<DeleteMcpServerCommandResult>;
    public record DeleteMcpServerCommandResult(bool result);    

    internal class DeleteMcpServerCommandHandler(IRepository<McpServer> repository, IMapper mapper) 
        : ICommandHandler<DeleteMcpServerCommand, DeleteMcpServerCommandResult>
    {
        private readonly IRepository<McpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<DeleteMcpServerCommandResult> Handle(DeleteMcpServerCommand request, CancellationToken cancellationToken)
        {
            repository.Delete(request.Id);
            await repository.SaveChangesAsync(cancellationToken);
            return new DeleteMcpServerCommandResult(true);
        }
    }
}
