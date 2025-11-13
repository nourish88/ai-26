using Juga.Abstractions.Data.AuditProperties;
using Juga.Abstractions.Data.Entities;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class LlmProvider : Entity<long>,IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public string Name {  get; set; }
        public List<Llm> Llms { get; set; }
        public List<Embedding> Embeddings { get; set; }
    }
}
