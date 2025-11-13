using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    internal class ExtractorEngineTypeConfiguration : IEntityTypeConfiguration<ExtractorEngineType>
    {
        public void Configure(EntityTypeBuilder<ExtractorEngineType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.HasMany(x => x.ApplicationExtractorEngines).WithOne(y => y.ExtractorEngineType).HasForeignKey(x => x.ExtractorEngineTypeId);

            builder.HasData(
                    new ExtractorEngineType()
                    {
                        Id = 1,
                        Identifier = ExtractorIdentifiers.MARKITDOWN,
                        Pdf=true,
                        Txt=true,
                        Word=true,
                    }
                );
        }
    }
}
