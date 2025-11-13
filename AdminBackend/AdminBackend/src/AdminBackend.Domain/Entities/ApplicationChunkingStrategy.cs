using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class ApplicationChunkingStrategy : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public long ApplicationId {  get; set; }
        public long ChunkingStrategyId { get; set; }
        public int? ChunkSize { get; set; }
        public int? Overlap { get; set; }
        public string? Seperator { get; set; }
        public Application Application { get; set; }
        public ChunkingStrategy ChunkingStrategy { get; set; }
    }
}
