using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.FileTypeHandlers
{

    public record CreateFileTypeCommand(
        string Identifier
        ) : ICommand<CreateFileTypeCommandResult>;
    public record CreateFileTypeCommandResult(
        long Id,
        string Identifier
        );

    public class CreateFileTypeCommandValidator : AbstractValidator<CreateFileTypeCommand>
    {
        public CreateFileTypeCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotNull().NotEmpty().MaximumLength(50);
        }
    }

    internal class CreateFileTypeCommandHandler(IRepository<FileType> repository, IMapper mapper) 
        : ICommandHandler<CreateFileTypeCommand, CreateFileTypeCommandResult>
    {
        private readonly IRepository<FileType> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateFileTypeCommandResult> Handle(CreateFileTypeCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<FileType>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateFileTypeCommandResult>(entitiy);
            return result;
        }
    }
}
