using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class ChunkingStrategy : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public string Identifier { get; set; }
        public bool IsChunkingSizeRequired { get; set; }
        public bool IsOverlapRequired { get; set; }
        public List<ApplicationChunkingStrategy> ApplicationChunkingStrategies { get; set; }
    }
}
