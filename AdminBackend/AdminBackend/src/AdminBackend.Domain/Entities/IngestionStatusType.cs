using AdminBackend.Domain.Constants;
using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class IngestionStatusType : Entity<IngestionStatusTypes>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public string Identifier {  get; set; }
        public List<File> Files { get; set; }
    }
}
