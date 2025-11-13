using System.Collections.Frozen;
using System.Text.Encodings.Web;
using AdminBackend.Application.Features.Ingestion;
using AdminBackend.Application.Repositories;
using AdminBackend.Application.Services.FileStorage;
using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using FluentValidation;
using Juga.CQRS.Abstractions;
using Juga.CrossCuttingConcerns.Exceptions.Types;
using Juga.Data.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using File = AdminBackend.Domain.Entities.File;

namespace AdminBackend.Application.Features.FileManagement.FileHandlers;

internal static class AllowedFileTypes
{
    public static readonly FrozenSet<string> ContentTypes =
        ContentTypeExtensionMapping.Keys.ToFrozenSet();

    public static FrozenDictionary<string, string> ContentTypeExtensionMapping =>
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "application/pdf", "pdf" },
            { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx" },
            { "text/plain", "txt" },
        }.ToFrozenDictionary();
}

public class UploadedFile
{
    public IFormFile File { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}

public class UploadRequest
{    
    public IReadOnlyList<UploadedFile> Files { get; set; }
}

public class UploadApplicationFileRequest : UploadRequest
{
    public string ApplicationIdentifier { get; set; }
}

public record UploadFilesCommand(
    string ApplicationIdentifier,
    FileTypes FileType,
    IReadOnlyList<UploadedFile> Files
) : ICommand<IEnumerable<UploadFilesCommandResult>>;

public record UploadFilesCommandResult(string DocumentId, string OriginalFilename);

public class FormFileValidator : AbstractValidator<UploadedFile>
{
    public FormFileValidator()
    {
        RuleFor(x => x.File).NotNull().NotEmpty();
        RuleFor(x => x.File.FileName).NotNull().NotEmpty();
        RuleFor(x => x.File.Length).NotEqual(0);
        RuleFor(x => x.File.ContentType)
            .NotNull()
            .NotEmpty()
            .Must(value => AllowedFileTypes.ContentTypes.Contains(value));
    }
}

public class UploadFilesCommandValidator : AbstractValidator<UploadFilesCommand>
{
    public UploadFilesCommandValidator()
    {
        RuleFor(x => x.ApplicationIdentifier).NotEmpty();
        RuleFor(x => x.FileType).IsInEnum();
        RuleFor(x => x.Files).NotNull().NotEmpty();
        RuleForEach(x => x.Files).SetValidator(new FormFileValidator());
    }
}

public class UploadFilesCommandHandler(
    IRepository<ApplicationFileStore> applicationFileStoreRepository,
    IRepository<File> fileRepository,
    IFileStorage fileStorage,
    ISender sender,
    ILogger<UploadFilesCommandHandler> logger,
    IApplicationRepository applicationRepository)
    : ICommandHandler<UploadFilesCommand, IEnumerable<UploadFilesCommandResult>>
{
    private async Task<IReadOnlyList<File>> UploadFiles(
        UploadFilesCommand command,
        CancellationToken cancellationToken)
    {
        var applicationEntity = await applicationRepository.GetByIdentifierAsync(command.ApplicationIdentifier);
        if (applicationEntity == null)
        {
            var message = $"Application({command.ApplicationIdentifier}) not found.";
            logger.LogError(message);
            throw new BusinessException(message);
        }

        if (!applicationEntity.HasApplicationFile && command.FileType == FileTypes.Application)
        {
            var message = $"The \"has application file\" feature is not available for the {command.ApplicationIdentifier} app. The application file cannot be uploaded.";
            logger.LogError(message);
            throw new BusinessException(message);
        }

        if (!applicationEntity.HasUserFile && command.FileType == FileTypes.Personal)
        {
            var message = $"The \"has user file\" feature is not available for the {command.ApplicationIdentifier} app. The user file cannot be uploaded.";
            logger.LogError(message);
            throw new BusinessException(message);
        }

        var fileStoreEntity = await applicationFileStoreRepository
            .AsNoTracking()
            .Where(x => x.ApplicationId == applicationEntity.Id)
            .Include(x => x.FileStore)
            .FirstOrDefaultAsync(cancellationToken);

        if (fileStoreEntity == null)
        {
            var message = $"Application({command.ApplicationIdentifier}) does not have a file store configured";
            logger.LogError(message);
            throw new BusinessException(message);
        }

        var storeEntityIdentifier = fileStoreEntity.FileStore.Identifier;

        var entities = command.Files.Select(x =>
        {
            var documentId = Guid.NewGuid().ToString();
            var fileExtension = AllowedFileTypes.ContentTypeExtensionMapping[x.File.ContentType];

            return new File
            {
                FileName = HtmlEncoder.Default.Encode(Path.GetFileName(x.File.FileName)),
                FileExtension = fileExtension,
                FileStoreId = fileStoreEntity.FileStoreId,
                FileStoreIdentifier = documentId,
                FileTypeId = command.FileType,
                UploadApplicationId = applicationEntity.Id,
                Title = x.Title,
                Description = x.Description,
                IngestionStatusTypeId = IngestionStatusTypes.ProcessingRequested
            };
        }).ToList().AsReadOnly();

        var fileStoreTasks = new List<Task>(command.Files.Count);

        for (int i = 0; i < command.Files.Count; i++)
        {
            var file = command.Files[i];
            var fileEntity = entities[i];
            var stream = file.File.OpenReadStream();
            var key =
                $"{fileEntity.FileStoreIdentifier}/raw/{fileEntity.FileStoreIdentifier}.{fileEntity.FileExtension}";
            fileStoreTasks.Add(fileStorage.PutObjectAsync(storeEntityIdentifier, key, stream, cancellationToken));
        }

        try
        {
            await Task.WhenAll(fileStoreTasks);
        }
        catch
        {
            var deletedFiles = entities
                .Select(x => $"{x.FileStoreIdentifier}/raw/{x.FileStoreIdentifier}.{x.FileExtension}")
                .ToList();
            try
            {
                await fileStorage.DeleteObjectsAsync(storeEntityIdentifier, deletedFiles, cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unable to delete files from storage");
            }

            throw;
        }

        return entities;
    }

    public async Task<IEnumerable<UploadFilesCommandResult>> Handle(
        UploadFilesCommand request,
        CancellationToken cancellationToken)
    {
        // Upload files to file storage
        var entities = await UploadFiles(request, cancellationToken);
        
        // Save them on DB's File table
        await fileRepository.InsertAsync(entities, cancellationToken);
        await fileRepository.SaveChangesAsync(cancellationToken);

        // Send extraction/chunking job requests to DataManager 
        foreach (var entity in entities)
        {
            await sender.Send(new SendJobRequestCommand(entity), cancellationToken);
        }

        return entities.Select(x => new UploadFilesCommandResult
        (
            x.FileStoreIdentifier, x.FileName
        ));
    }
}