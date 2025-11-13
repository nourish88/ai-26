using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers
{
    public record UpdateFileCommand(
        long Id,
        DateTime? CreatedDate,
        string? CreatedBy,
        string Title,
        string FileName,
        string FileExtension,
        long FileStoreId,
        string FileStoreIdentifier,
        string? Description,
        long UploadApplicationId,
        IngestionStatusTypes IngestionStatusTypeId,
        FileTypes FileTypeId,
        string? ErrorDetail
        ) : ICommand<UpdateFileCommandResult>;
    public record UpdateFileCommandResult(
        long Id,
        DateTime? CreatedDate,
        string? CreatedBy,
        string Title,
        string FileName,
        string FileExtension,
        long FileStoreId,
        string FileStoreIdentifier,
        string? Description,
        long UploadApplicationId,
        IngestionStatusTypes IngestionStatusTypeId,
        FileTypes FileTypeId,
        string? ErrorDetail
        ) ;

    public class UpdateFileCommandValidator : AbstractValidator<UpdateFileCommand>
    {
        public UpdateFileCommandValidator()
        {
            RuleFor(x => x.Title).MaximumLength(50);
            RuleFor(x => x.FileName).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x => x.FileExtension).NotNull().NotEmpty().MaximumLength(10);
            RuleFor(x => x.FileStoreIdentifier).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.Description).MaximumLength(2000);
        }
    }
    internal class UpdateFileCommandHandler(IRepository<Domain.Entities.File> repository, IMapper mapper)
        : ICommandHandler<UpdateFileCommand, UpdateFileCommandResult>
    {
        private readonly IRepository<Domain.Entities.File> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<UpdateFileCommandResult> Handle(UpdateFileCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Domain.Entities.File>(request);
            repository.Update(entitiy);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<UpdateFileCommandResult>(entitiy);
            return result;
        }
    }
}
