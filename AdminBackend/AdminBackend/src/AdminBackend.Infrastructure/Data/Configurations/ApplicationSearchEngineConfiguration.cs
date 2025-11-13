using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ApplicationSearchEngineConfiguration : IEntityTypeConfiguration<ApplicationSearchEngine>
    {
        public void Configure(EntityTypeBuilder<ApplicationSearchEngine> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.IndexName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.HasOne(x => x.Application);
            builder.HasOne(x => x.SearchEngine);
            builder.HasOne(x => x.Embedding);

            builder.HasData(            
                new ApplicationSearchEngine()
                {
                    Id = 1,
                    ApplicationId = 1,
                    SearchEngineId = 1,
                    IndexName = "test-app-index",
                    Identifier = "test-app-local-index",
                    EmbeddingId = 3,
                    VectorSize = 2560
                }
            );
        }
    }
}
