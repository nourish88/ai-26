using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class Embedding : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public long LlmProviderId { get; set; }
        public string Url { get; set; }
        public string ModelName { get; set; }
        public int VectorSize { get; set; }
        public int MaxInputTokenSize { get; set; }
        public LlmProvider LlmProvider { get; set; }
        public List<ApplicationSearchEngine> ApplicationSearchEngines { get; set; }
    }
}
