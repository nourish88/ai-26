using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.ApplicationFileStoreHandlers
{
    public record UpdateApplicationFileStoreCommand(
        long Id,
        long ApplicationId,
        long FileStoreId
        ) : ICommand<UpdateApplicationFileStoreCommandResult>;
    public record UpdateApplicationFileStoreCommandResult(
        long Id,
        long ApplicationId,
        long FileStoreId
        ) ;

    public class UpdateApplicationFileStoreCommandValidator : AbstractValidator<UpdateApplicationFileStoreCommand>
    {
        public UpdateApplicationFileStoreCommandValidator()
        {
            //TODO: check applicationId and FileStoreId uniqueness
        }
    }
    internal class UpdateApplicationFileStoreCommandHandler(IRepository<ApplicationFileStore> repository, IMapper mapper)
        : ICommandHandler<UpdateApplicationFileStoreCommand, UpdateApplicationFileStoreCommandResult>
    {
        private readonly IRepository<ApplicationFileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateApplicationFileStoreCommandResult> Handle(UpdateApplicationFileStoreCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationFileStore>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateApplicationFileStoreCommandResult>(entitiy);
            return result;
        }
    }
}
