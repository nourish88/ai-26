using AdminBackend.Domain.Constants;

namespace AdminBackend.Application.Dtos
{
    public record FileDto
    (
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
}
