using AdminBackend.Domain.Constants;
using AutoMapper;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.Data.Abstractions;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers
{

    public record CreateFileCommand(
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
        ) : ICommand<CreateFileCommandResult>;
    
    public record CreateFileCommandResult(
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
        );

    public class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
    {
        public CreateFileCommandValidator()
        {
            RuleFor(x=>x.Title).MaximumLength(50);
            RuleFor(x=>x.FileName).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x=>x.FileExtension).NotNull().NotEmpty().MaximumLength(10);
            RuleFor(x=>x.FileStoreIdentifier).NotNull().NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.Description).MaximumLength(2000);
        }
    }

    internal class CreateFileCommandHandler(IRepository<Domain.Entities.File> repository, IMapper mapper) 
        : ICommandHandler<CreateFileCommand, CreateFileCommandResult>
    {
        private readonly IRepository<Domain.Entities.File> repository = repository;
        private readonly IMapper mapper = mapper;

        public async Task<CreateFileCommandResult> Handle(CreateFileCommand request, CancellationToken cancellationToken)
        {
            var entitiy = mapper.Map<Domain.Entities.File>(request);
            entitiy = await repository.InsertAsync(entitiy,cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            var result = mapper.Map<CreateFileCommandResult>(entitiy);
            return result;
        }
    }
}
