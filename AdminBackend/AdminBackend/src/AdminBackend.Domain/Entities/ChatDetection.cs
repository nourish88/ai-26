using AdminBackend.Domain.Constants;
using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class ChatDetection : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public string ApplicationIdentifier { get; set; }
        public ChatDetectionTypes ChatDetectionTypeId {  get; set; }
        public string ThreadId {  get; set; }
        public string MessageId { get; set; }
        public string UserMessage { get; set; }
        public string? Sources { get; set; }
        public string? Reason { get; set; }
        
    }
}
