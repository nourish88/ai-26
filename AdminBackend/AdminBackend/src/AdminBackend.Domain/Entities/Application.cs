using AdminBackend.Domain.Constants;
using Juga.Abstractions.Data.AuditProperties;
using Juga.Domain.Base;

namespace AdminBackend.Domain.Entities
{
    public class Application : Entity<long>, IHasFullAudit
    {
        public new DateTime? CreatedDate { get; set; }
        public new string? CreatedBy { get; set; }
        public new string? CreatedAt { get; set; }
        public string Name { get; set; }
        public string Identifier { get; set; }
        public bool HasApplicationFile { get; set; }
        public bool HasUserFile { get; set; }
        public bool EnableGuardRails { get; set; }
        public bool CheckHallucination { get; set; }
        public string Description { get; set; }
        public string SystemPrompt {  get; set; }
        public ApplicationTypes ApplicationTypeId { get; set; }
        public MemoryTypes MemoryTypeId {  get; set; }
        public OutputTypes OutputTypeId { get; set; }
        public ApplicationChunkingStrategy ApplicationChunkingStrategy { get; set; }
        public ApplicationExtractorEngine ApplicationExtractorEngine { get; set; }
        public ApplicationFileStore ApplicationFileStore { get; set; }
        public ApplicationSearchEngine ApplicationSearchEngine { get; set; }
        public ApplicationLlm ApplicationLlm { get; set; }
        public ApplicationEmbedding ApplicationEmbedding { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public MemoryType MemoryType { get; set; }
        public OutputType OutputType { get; set; }
        public List<ApplicationMcpServer> ApplicationMcpServers { get; set; }
    }
}
