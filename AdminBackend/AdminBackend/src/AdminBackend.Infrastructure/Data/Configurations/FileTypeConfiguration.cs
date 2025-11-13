using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class FileTypeConfiguration : IEntityTypeConfiguration<FileType>
    {
        public void Configure(EntityTypeBuilder<FileType> builder)
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
            builder.HasMany(x => x.Files).WithOne(y => y.FileType).HasForeignKey(x => x.FileTypeId);

            builder.HasData(
                new FileType()
                {
                    Id = FileTypes.Personal,
                    Identifier = nameof(FileTypes.Personal)
                },
                new FileType()
                {
                    Id = FileTypes.Application,
                    Identifier = nameof(FileTypes.Application)
                }

            );
        }
    }
}
