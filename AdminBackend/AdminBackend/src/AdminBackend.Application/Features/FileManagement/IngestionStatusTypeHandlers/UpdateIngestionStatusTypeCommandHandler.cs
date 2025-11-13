using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.IngestionStatusTypeHandlers
{
    public record UpdateIngestionStatusTypeCommand(
        long Id,
        string Identifier
        ) : ICommand<UpdateIngestionStatusTypeCommandResult>;
    public record UpdateIngestionStatusTypeCommandResult(
        long Id,
        string Identifier
        ) ;

    public class UpdateIngestionStatusTypeCommandValidator : AbstractValidator<UpdateIngestionStatusTypeCommand>
    {
        public UpdateIngestionStatusTypeCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotNull().NotEmpty().MaximumLength(50);
        }
    }
    internal class UpdateIngestionStatusTypeCommandHandler(IRepository<IngestionStatusType> repository, IMapper mapper)
        : ICommandHandler<UpdateIngestionStatusTypeCommand, UpdateIngestionStatusTypeCommandResult>
    {
        private readonly IRepository<IngestionStatusType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateIngestionStatusTypeCommandResult> Handle(UpdateIngestionStatusTypeCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<IngestionStatusType>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateIngestionStatusTypeCommandResult>(entitiy);
            return result;
        }
    }
}
