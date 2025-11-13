using AdminBackend.Domain.Constants;
using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class File : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public string? Title {  get; set; }
        public string FileName {  get; set; }
        public string FileExtension {  get; set; }
        public long FileStoreId { get; set; }
        public string FileStoreIdentifier { get; set; }
        public string? Description { get; set; }
        public long UploadApplicationId {  get; set; }
        public IngestionStatusTypes IngestionStatusTypeId {  get; set; }
        public FileTypes FileTypeId { get; set; }
        public string? ErrorDetail { get; set; }
        public FileStore? FileStore { get; set; }
        public IngestionStatusType? IngestionStatusType { get; set; }
        public FileType? FileType { get; set; }
    }
}
