using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationMcpServerHandlers
{

    public record CreateApplicationMcpServerCommand(
        long ApplicationId,
        long McpServerId) : ICommand<CreateApplicationMcpServerCommandResult>;
    public record CreateApplicationMcpServerCommandResult(
        long Id,
        long ApplicationId,
        long McpServerId);

    public class CreateApplicationMcpServerCommandValidator : AbstractValidator<CreateApplicationMcpServerCommand>
    {
        public CreateApplicationMcpServerCommandValidator()
        {
            //TODO : Unique check
        }
    }

    internal class CreateApplicationMcpServerCommandHandler(IRepository<ApplicationMcpServer> repository, IMapper mapper) 
        : ICommandHandler<CreateApplicationMcpServerCommand, CreateApplicationMcpServerCommandResult>
    {
        private readonly IRepository<ApplicationMcpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateApplicationMcpServerCommandResult> Handle(CreateApplicationMcpServerCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationMcpServer>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateApplicationMcpServerCommandResult>(entitiy);
            return result;
        }
    }
}
