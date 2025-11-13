using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.ApplicationFileStoreHandlers
{

    public record CreateApplicationFileStoreCommand(
        long ApplicationId,
        long FileStoreId
        ) : ICommand<CreateApplicationFileStoreCommandResult>;
    public record CreateApplicationFileStoreCommandResult(
        long Id,
        long ApplicationId,
        long FileStoreId
        );

    public class CreateApplicationFileStoreCommandValidator : AbstractValidator<CreateApplicationFileStoreCommand>
    {
        public CreateApplicationFileStoreCommandValidator()
        {
            //TODO: check applicationId and FileStoreId uniqueness
        }
    }

    internal class CreateApplicationFileStoreCommandHandler(IRepository<ApplicationFileStore> repository, IMapper mapper) 
        : ICommandHandler<CreateApplicationFileStoreCommand, CreateApplicationFileStoreCommandResult>
    {
        private readonly IRepository<ApplicationFileStore> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateApplicationFileStoreCommandResult> Handle(CreateApplicationFileStoreCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<ApplicationFileStore>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateApplicationFileStoreCommandResult>(entitiy);
            return result;
        }
    }
}
