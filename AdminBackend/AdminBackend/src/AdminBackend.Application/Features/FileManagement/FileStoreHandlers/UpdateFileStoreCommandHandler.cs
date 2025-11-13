using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.FileStoreHandlers
{
    public record UpdateFileStoreCommand(
        long Id,
        string Identifier,
        string Uri
        ) : ICommand<UpdateFileStoreCommandResult>;
    public record UpdateFileStoreCommandResult(
        long Id,
        string Identifier,
        string Uri
        ) ;

    public class UpdateFileStoreCommandValidator : AbstractValidator<UpdateFileStoreCommand>
    {
        public UpdateFileStoreCommandValidator()
        {
            //TODO : Unique check
            RuleFor(x => x.Identifier).NotNull().NotEmpty().MaximumLength(50);
            RuleFor(x => x.Uri).NotNull().NotEmpty().MaximumLength(250);
        }
    }
    internal class UpdateFileStoreCommandHandler(IRepository<FileStore> repository, IMapper mapper)
        : ICommandHandler<UpdateFileStoreCommand, UpdateFileStoreCommandResult>
    {
        private readonly IRepository<FileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateFileStoreCommandResult> Handle(UpdateFileStoreCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<FileStore>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateFileStoreCommandResult>(entitiy);
            return result;
        }
    }
}
