using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class ExtractorEngineType : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public string Identifier {  get; set; }
        public bool Word {  get; set; }
        public bool Txt {  get; set; }
        public bool Pdf { get; set; }
        public List<ApplicationExtractorEngine> ApplicationExtractorEngines { get; set; }
    }
}
