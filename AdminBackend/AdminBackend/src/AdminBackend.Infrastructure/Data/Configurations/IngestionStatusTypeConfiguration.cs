using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class IngestionStatusTypeConfiguration : IEntityTypeConfiguration<IngestionStatusType>
    {
        public void Configure(EntityTypeBuilder<IngestionStatusType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedNever()
                .IsRequired()
                .HasConversion<int>();
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.HasMany(x => x.Files).WithOne(y => y.IngestionStatusType)
                .HasForeignKey(x => x.IngestionStatusTypeId);
            builder.HasData(
                new IngestionStatusType
                {
                    Id = IngestionStatusTypes.ProcessingRequested,
                    Identifier = nameof(IngestionStatusTypes.ProcessingRequested)
                },
                new IngestionStatusType()
                {
                    Id = IngestionStatusTypes.Extracting,
                    Identifier = nameof(IngestionStatusTypes.Extracting)
                },
                new IngestionStatusType()
                {
                    Id = IngestionStatusTypes.Chunking,
                    Identifier = nameof(IngestionStatusTypes.Chunking)
                },
                new IngestionStatusType()
                {
                    Id = IngestionStatusTypes.Indexing,
                    Identifier = nameof(IngestionStatusTypes.Indexing)
                },
                new IngestionStatusType()
                {
                    Id = IngestionStatusTypes.Processed,
                    Identifier = nameof(IngestionStatusTypes.Processed)
                },
                new IngestionStatusType()
                {
                    Id = IngestionStatusTypes.DeletingRequested,
                    Identifier = nameof(IngestionStatusTypes.DeletingRequested)
                },
                new IngestionStatusType()
                {
                    Id = IngestionStatusTypes.ProcessingFailed,
                    Identifier = nameof(IngestionStatusTypes.ProcessingFailed)
                }
            );
        }
    }
}