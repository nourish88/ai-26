using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class OutputTypeConfiguration : IEntityTypeConfiguration<OutputType>
    {
        public void Configure(EntityTypeBuilder<OutputType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedNever()
                .IsRequired()
                .HasConversion<long>();
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.HasMany(x => x.Applications).WithOne(y => y.OutputType).HasForeignKey(x => x.OutputTypeId);
            builder.HasData(
                new OutputType()
                {
                    Id = Domain.Constants.OutputTypes.BLOCK,
                    Identifier=nameof(Domain.Constants.OutputTypes.BLOCK)
                },
                new OutputType()
                {
                    Id = Domain.Constants.OutputTypes.STREAMING,
                    Identifier = nameof(Domain.Constants.OutputTypes.STREAMING)
                },
                new OutputType()
                {
                    Id = Domain.Constants.OutputTypes.BOTH,
                    Identifier = nameof(Domain.Constants.OutputTypes.BOTH)
                }
            );
        }
    }
}
