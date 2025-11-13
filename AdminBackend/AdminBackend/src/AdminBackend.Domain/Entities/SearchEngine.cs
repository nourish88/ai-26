using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class SearchEngine : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public long SearchEngineTypeId { get; set; }
        public string Url { get; set; }
        public string Identifier { get; set; }
        public SearchEngineType SearchEngineType { get; set; }
        public List<ApplicationSearchEngine> ApplicationSearchEngines { get; set; }
    }
}
