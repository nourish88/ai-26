using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.FileTypeHandlers
{
    public record UpdateFileTypeCommand(
        long Id,
        string Identifier
        ) : ICommand<UpdateFileTypeCommandResult>;
    public record UpdateFileTypeCommandResult(
        long Id,
        string Identifier
        ) ;

    public class UpdateFileTypeCommandValidator : AbstractValidator<UpdateFileTypeCommand>
    {
        public UpdateFileTypeCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotNull().NotEmpty().MaximumLength(50);
        }
    }
    internal class UpdateFileTypeCommandHandler(IRepository<FileType> repository, IMapper mapper)
        : ICommandHandler<UpdateFileTypeCommand, UpdateFileTypeCommandResult>
    {
        private readonly IRepository<FileType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateFileTypeCommandResult> Handle(UpdateFileTypeCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<FileType>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateFileTypeCommandResult>(entitiy);
            return result;
        }
    }
}
