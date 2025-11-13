using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class FileConfiguration : IEntityTypeConfiguration<Domain.Entities.File>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.File> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Title).HasMaxLength(50);
            builder.Property(x => x.FileName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.FileExtension).IsRequired().HasMaxLength(10);
            builder.Property(x => x.FileStoreId).IsRequired(true);
            builder.Property(x => x.FileStoreIdentifier).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x=>x.ErrorDetail).HasMaxLength(2000);
            builder.HasOne(x => x.FileStore);
            builder.HasOne(x => x.IngestionStatusType);
            builder.HasOne(x => x.FileType);
        }
    }
}
