using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class ApplicationEmbedding : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public long ApplicationId { get; set; }
        public long EmbeddingId { get; set; }
        public Application Application { get; set; }
        public Embedding Embedding { get; set; }
    }
}
