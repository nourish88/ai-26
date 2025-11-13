using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace ToolGateway.Domain.Entities
{
    public class Todo : Entity<Guid>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsCompleted {  get; set; }
    }
}
