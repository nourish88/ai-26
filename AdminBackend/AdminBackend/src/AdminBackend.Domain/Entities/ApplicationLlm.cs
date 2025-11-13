using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class ApplicationLlm : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public float TopP { get; set; }
        public float Temperature { get; set; }
        public bool EnableThinking { get; set; }
        public long ApplicationId { get; set; }
        public long LlmId { get; set; }
        public Application Application { get; set; }
        public Llm Llm { get; set; }
    }
}
