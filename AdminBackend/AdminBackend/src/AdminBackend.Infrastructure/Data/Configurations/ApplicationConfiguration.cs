using AdminBackend.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Domain.Entities.Application>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Application> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.Property(x => x.Description).IsRequired().HasMaxLength(255);
            builder.Property(x => x.SystemPrompt).IsRequired().HasMaxLength(2000);
            builder.HasOne(x => x.ApplicationChunkingStrategy);
            builder.HasOne(x => x.ApplicationExtractorEngine);
            builder.HasOne(x => x.ApplicationFileStore);
            builder.HasOne(x => x.ApplicationSearchEngine);
            builder.HasOne(x => x.ApplicationLlm);
            builder.HasOne(x => x.ApplicationEmbedding);
            builder.HasOne(x => x.ApplicationType);
            builder.HasOne(x => x.MemoryType);
            builder.HasOne(x => x.OutputType);
            builder.HasMany(x => x.ApplicationMcpServers).WithOne(y => y.Application).HasForeignKey(f => f.ApplicationId);

            builder.HasData(
                new Domain.Entities.Application()
                {
                    Id = 1,
                    HasApplicationFile = true,
                    HasUserFile = false,
                    EnableGuardRails = true,
                    CheckHallucination = true,
                    Identifier = "test-app",
                    Name = "Test App",
                    Description = "This is a test application for the AI Orchestrator.",
                    SystemPrompt = "You are a helpful AI assistant.",
                    ApplicationTypeId = ApplicationTypes.REACT,
                    MemoryTypeId = MemoryTypes.MONGO,
                    OutputTypeId = OutputTypes.BOTH,
                }
            );
        }
    }
}
