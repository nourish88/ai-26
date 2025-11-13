using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ApplicationChunkingStrategyConfiguration : IEntityTypeConfiguration<ApplicationChunkingStrategy>
    {
        public void Configure(EntityTypeBuilder<ApplicationChunkingStrategy> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.ApplicationId).IsRequired();
            builder.Property(x => x.ChunkingStrategyId).IsRequired();
            builder.HasOne(x => x.Application);
            builder.HasOne(x => x.ChunkingStrategy);

            builder.HasData(new ApplicationChunkingStrategy
            {
                Id = 1,
                ChunkSize = 2000,
                Overlap = 200,
                ApplicationId = 1,
                ChunkingStrategyId = 3,
                Seperator = null
            });
        }
    }
}
