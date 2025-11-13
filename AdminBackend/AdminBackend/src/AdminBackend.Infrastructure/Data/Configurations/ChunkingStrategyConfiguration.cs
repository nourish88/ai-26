using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ChunkingStrategyConfiguration : IEntityTypeConfiguration<ChunkingStrategy>
    {
        public void Configure(EntityTypeBuilder<ChunkingStrategy> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.HasMany(x => x.ApplicationChunkingStrategies).WithOne(y=>y.ChunkingStrategy).HasForeignKey(k=>k.ChunkingStrategyId);

            builder.HasData(
                    new ChunkingStrategy()
                    {
                        Id = 1,
                        Identifier = ChunkingStrategyIdentifiers.FIXED_SIZE,
                        IsChunkingSizeRequired = true,
                        IsOverlapRequired = true,
                    },
                    new ChunkingStrategy()
                    {
                        Id = 2,
                        Identifier = ChunkingStrategyIdentifiers.HTML,
                        IsChunkingSizeRequired = true,
                        IsOverlapRequired = true,
                    },
                    new ChunkingStrategy()
                    {
                        Id = 3,
                        Identifier = ChunkingStrategyIdentifiers.MARKDOWN,
                        IsChunkingSizeRequired = true,
                        IsOverlapRequired = true,
                    }
                );
        }
    }
}
