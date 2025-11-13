using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationMcpServerHandlers
{
    public record UpdateApplicationMcpServerCommand(
        long Id,
        long ApplicationId,
        long McpServerId) : ICommand<UpdateApplicationMcpServerCommandResult>;
    public record UpdateApplicationMcpServerCommandResult(
        long Id,
        long ApplicationId,
        long McpServerId) ;

    public class UpdateApplicationMcpServerCommandValidator : AbstractValidator<UpdateApplicationMcpServerCommand>
    {
        public UpdateApplicationMcpServerCommandValidator()
        {
            //TODO : Unique check
        }
    }
    internal class UpdateApplicationMcpServerCommandHandler(IRepository<ApplicationMcpServer> repository, IMapper mapper)
        : ICommandHandler<UpdateApplicationMcpServerCommand, UpdateApplicationMcpServerCommandResult>
    {
        private readonly IRepository<ApplicationMcpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateApplicationMcpServerCommandResult> Handle(UpdateApplicationMcpServerCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationMcpServer>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateApplicationMcpServerCommandResult>(entitiy);
            return result;
        }
    }
}
