using AdminBackend.Domain.Constants;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationHandlers
{

    public record CreateApplicationCommand(
        string Name,
        string Identifier,
        bool HasApplicationFile,
        bool HasUserFile,
        bool EnableGuardRails,
        bool CheckHallucination,
        string Description,
        string SystemPrompt,
        ApplicationTypes ApplicationTypeId,
        MemoryTypes MemoryTypeId,
        OutputTypes OutputTypeId) : ICommand<CreateApplicationCommandResult>;
    public record CreateApplicationCommandResult(
        long Id,
        string Name,
        string Identifier,
        bool HasApplicationFile,
        bool HasUserFile,
        bool EnableGuardRails,
        bool CheckHallucination,
        string Description,
        string SystemPrompt,
        ApplicationTypes ApplicationTypeId,
        MemoryTypes MemoryTypeId,
        OutputTypes OutputTypeId);

    public class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
    {
        public CreateApplicationCommandValidator()
        {
            RuleFor(x=>x.Name).NotNull().NotEmpty().MaximumLength(50);
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x => x.SystemPrompt).NotNull().NotEmpty().MaximumLength(2000);
        }
    }

    internal class CreateApplicationCommandHandler(IRepository<Domain.Entities.Application> repository, IMapper mapper) 
        : ICommandHandler<CreateApplicationCommand, CreateApplicationCommandResult>
    {
        private readonly IRepository<Domain.Entities.Application> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateApplicationCommandResult> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Domain.Entities.Application>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateApplicationCommandResult>(entitiy);
            return result;
        }
    }
}
