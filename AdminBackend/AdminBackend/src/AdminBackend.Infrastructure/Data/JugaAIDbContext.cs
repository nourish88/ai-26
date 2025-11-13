using AdminBackend.Domain.Entities;
using Juga.Abstractions.Data.AuditLog;
using Juga.Data;
using Juga.Data.AuditProperties;
using Juga.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace AdminBackend.Infrastructure.Data
{
    public class JugaAIDbContext : UnitOfWork
    {
        public JugaAIDbContext(
        IOptions<UnitOfWorkOptions> options,
        IAuditPropertyInterceptorManager auditPropertyInterceptorManager,
        DbContextOptions<JugaAIDbContext> dbContextOptions,
        IAuditBehaviourService auditBehaviourService,
        IConfiguration configuration)
        : base(options, dbContextOptions, auditPropertyInterceptorManager, auditBehaviourService, configuration)
        {



        }
        public JugaAIDbContext(DbContextOptions<JugaAIDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<LlmProvider> LlmProviders => Set<LlmProvider>();
        public DbSet<Llm> Llms => Set<Llm>();
        public DbSet<Embedding> Embeddings => Set<Embedding>();
        public DbSet<ChunkingStrategy> ChunkingStrategies => Set<ChunkingStrategy>();
        public DbSet<ApplicationType> ApplicationTypes => Set<ApplicationType>();
        public DbSet<MemoryType> MemoryTypes => Set<MemoryType>();
        public DbSet<OutputType> OutputTypes => Set<OutputType>();
        public DbSet<Domain.Entities.Application> Applications => Set<Domain.Entities.Application>();
        public DbSet<ApplicationChunkingStrategy> ApplicationChunkingStrategies => Set<ApplicationChunkingStrategy>();
        public DbSet<ExtractorEngineType> ExtractorEngineTypes => Set<ExtractorEngineType>();
        public DbSet<ApplicationExtractorEngine> ApplicationExtractorEngines => Set<ApplicationExtractorEngine>();
        public DbSet<IngestionStatusType> IngestionStatusTypes => Set<IngestionStatusType>();
        public DbSet<FileType> FileTypes => Set<FileType>();
        public DbSet<FileStore> FileStores => Set<FileStore>();
        public DbSet<ApplicationFileStore> ApplicationFileStores => Set<ApplicationFileStore>();
        public DbSet<Domain.Entities.File> Files => Set<Domain.Entities.File>();
        public DbSet<SearchEngineType> SearchEngineTypes => Set<SearchEngineType>();
        public DbSet<SearchEngine> SearchEngines => Set<SearchEngine>();
        public DbSet<ApplicationSearchEngine> ApplicationSearchEngines => Set<ApplicationSearchEngine>();
        public DbSet<ApplicationLlm> ApplicationLlms => Set<ApplicationLlm>();
        public DbSet<ApplicationEmbedding> ApplicationEmbeddings => Set<ApplicationEmbedding>();
        public DbSet<McpServer> McpServers => Set<McpServer>();
        public DbSet<ApplicationMcpServer> ApplicationMcpServers => Set<ApplicationMcpServer>();
        public DbSet<ChatDetection> ChatDetections => Set<ChatDetection>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("jugaai");// Data Isolaliton on Schema Level
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
