using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ApplicationEmbeddingConfiguration : IEntityTypeConfiguration<ApplicationEmbedding>
    {
        public void Configure(EntityTypeBuilder<ApplicationEmbedding> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.HasOne(x => x.Embedding);
            builder.HasOne(x => x.Application);
            builder.HasIndex(x => new { x.ApplicationId, x.EmbeddingId }).IsUnique();

            builder.HasData(
                new ApplicationEmbedding()
                {
                    Id = 1,
                    ApplicationId = 1,
                    EmbeddingId = 3,
                }
            );
        }
    }
}
