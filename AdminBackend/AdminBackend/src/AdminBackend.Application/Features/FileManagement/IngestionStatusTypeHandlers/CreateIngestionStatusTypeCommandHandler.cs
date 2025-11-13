using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.IngestionStatusTypeHandlers
{

    public record CreateIngestionStatusTypeCommand(
        string Identifier
        ) : ICommand<CreateIngestionStatusTypeCommandResult>;
    public record CreateIngestionStatusTypeCommandResult(
        long Id,
        string Identifier
        );

    public class CreateIngestionStatusTypeCommandValidator : AbstractValidator<CreateIngestionStatusTypeCommand>
    {
        public CreateIngestionStatusTypeCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotNull().NotEmpty().MaximumLength(50);
        }
    }

    internal class CreateIngestionStatusTypeCommandHandler(IRepository<IngestionStatusType> repository, IMapper mapper) 
        : ICommandHandler<CreateIngestionStatusTypeCommand, CreateIngestionStatusTypeCommandResult>
    {
        private readonly IRepository<IngestionStatusType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateIngestionStatusTypeCommandResult> Handle(CreateIngestionStatusTypeCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<IngestionStatusType>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateIngestionStatusTypeCommandResult>(entitiy);
            return result;
        }
    }
}
