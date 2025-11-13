using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class McpServer : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public string Identifier { get; set; }
        public string Uri { get; set; }
        public List<ApplicationMcpServer> ApplicationMcpServers { get; set; }
    }
}
