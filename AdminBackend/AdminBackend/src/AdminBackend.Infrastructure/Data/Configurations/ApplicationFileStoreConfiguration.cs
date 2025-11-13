using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ApplicationFileStoreConfiguration : IEntityTypeConfiguration<ApplicationFileStore>
    {
        public void Configure(EntityTypeBuilder<ApplicationFileStore> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.HasOne(x => x.Application);
            builder.HasOne(x => x.FileStore);
            builder.HasIndex(x => new { x.ApplicationId, x.FileStoreId }).IsUnique();

            builder.HasData([
                new ApplicationFileStore
                {
                    Id = 1,
                    ApplicationId = 1,
                    FileStoreId = 1,
                }
            ]);
        }
    }
}