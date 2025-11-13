using AdminBackend.Domain.Constants;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.ApplicationManagement.ApplicationHandlers
{
    public record UpdateApplicationCommand(
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
        OutputTypes OutputTypeId) : ICommand<UpdateApplicationCommandResult>;
    public record UpdateApplicationCommandResult(
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
        OutputTypes OutputTypeId) ;

    public class UpdateApplicationCommandValidator : AbstractValidator<UpdateApplicationCommand>
    {
        public UpdateApplicationCommandValidator()
        {
            RuleFor(x => x.Name).NotNull().NotEmpty().MaximumLength(50);
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x => x.SystemPrompt).NotNull().NotEmpty().MaximumLength(2000);
        }
    }
    internal class UpdateApplicationCommandHandler(IRepository<Domain.Entities.Application> repository, IMapper mapper)
        : ICommandHandler<UpdateApplicationCommand, UpdateApplicationCommandResult>
    {
        private readonly IRepository<Domain.Entities.Application> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateApplicationCommandResult> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Domain.Entities.Application>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateApplicationCommandResult>(entitiy);
            return result;
        }
    }
}
