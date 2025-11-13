using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.FileStoreHandlers
{

    public record CreateFileStoreCommand(
        string Identifier,
        string Uri
        ) : ICommand<CreateFileStoreCommandResult>;
    public record CreateFileStoreCommandResult(
        long Id,
        string Identifier,
        string Uri
        );

    public class CreateFileStoreCommandValidator : AbstractValidator<CreateFileStoreCommand>
    {
        public CreateFileStoreCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x=>x.Identifier).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x=>x.Uri).NotNull().NotEmpty().MaximumLength(250);
        }
    }

    internal class CreateFileStoreCommandHandler(IRepository<FileStore> repository, IMapper mapper) 
        : ICommandHandler<CreateFileStoreCommand, CreateFileStoreCommandResult>
    {
        private readonly IRepository<FileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateFileStoreCommandResult> Handle(CreateFileStoreCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<FileStore>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateFileStoreCommandResult>(entitiy);
            return result;
        }
    }
}
