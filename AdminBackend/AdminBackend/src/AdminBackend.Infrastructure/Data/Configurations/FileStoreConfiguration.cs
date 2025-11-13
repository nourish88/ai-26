using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class FileStoreConfiguration : IEntityTypeConfiguration<FileStore>
    {
        public void Configure(EntityTypeBuilder<FileStore> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.Property(x => x.Uri).IsRequired().HasMaxLength(250);
            builder.HasMany(x => x.ApplicationFileStores).WithOne(y => y.FileStore).HasForeignKey(x => x.FileStoreId);
            builder.HasMany(x => x.Files).WithOne(y => y.FileStore).HasForeignKey(x => x.FileStoreId);

            builder.HasData([
                new FileStore
                {
                    Id = 1,
                    Identifier = "s3",
                    Uri = "127.0.0.1:8090"
                }
            ]);
        }
    }
}