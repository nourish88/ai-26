using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.McpServerManagement.McpServerHandlers
{

    public record CreateMcpServerCommand(
        string Identifier,
        string Uri
        ) : ICommand<CreateMcpServerCommandResult>;
    public record CreateMcpServerCommandResult(
        long Id,
        string Identifier,
        string Uri
        );

    public class CreateMcpServerCommandValidator : AbstractValidator<CreateMcpServerCommand>
    {
        public CreateMcpServerCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x=>x.Uri).NotNull().NotEmpty().MaximumLength(250);
        }
    }

    internal class CreateMcpServerCommandHandler(IRepository<McpServer> repository, IMapper mapper) 
        : ICommandHandler<CreateMcpServerCommand, CreateMcpServerCommandResult>
    {
        private readonly IRepository<McpServer> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateMcpServerCommandResult> Handle(CreateMcpServerCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<McpServer>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateMcpServerCommandResult>(entitiy);
            return result;
        }
    }
}
