using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.McpServerManagement.McpServerHandlers
{
    public record UpdateMcpServerCommand(
        long Id,
        string Identifier,
        string Uri
        ) : ICommand<UpdateMcpServerCommandResult>;
    public record UpdateMcpServerCommandResult(
        long Id,
        string Identifier,
        string Uri
        ) ;

    public class UpdateMcpServerCommandValidator : AbstractValidator<UpdateMcpServerCommand>
    {
        public UpdateMcpServerCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.Uri).NotNull().NotEmpty().MaximumLength(250);
        }
    }
    internal class UpdateMcpServerCommandHandler(IRepository<McpServer> repository, IMapper mapper)
        : ICommandHandler<UpdateMcpServerCommand, UpdateMcpServerCommandResult>
    {
        private readonly IRepository<McpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateMcpServerCommandResult> Handle(UpdateMcpServerCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<McpServer>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateMcpServerCommandResult>(entitiy);
            return result;
        }
    }
}
