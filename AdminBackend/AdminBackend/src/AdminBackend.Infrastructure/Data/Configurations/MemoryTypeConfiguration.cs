using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class MemoryTypeConfiguration : IEntityTypeConfiguration<MemoryType>
    {
        public void Configure(EntityTypeBuilder<MemoryType> builder)
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
            builder.HasMany(x => x.Applications).WithOne(y => y.MemoryType).HasForeignKey(x => x.MemoryTypeId);

            builder.HasData(
                    new MemoryType()
                    {
                        Id = Domain.Constants.MemoryTypes.MEMORY,
                        Identifier = nameof(Domain.Constants.MemoryTypes.MEMORY)
                    },
                    new MemoryType()
                    {
                        Id = Domain.Constants.MemoryTypes.MONGO,
                        Identifier = nameof(Domain.Constants.MemoryTypes.MONGO)
                    },
                    new MemoryType()
                    {
                        Id = Domain.Constants.MemoryTypes.POSTGRE,
                        Identifier = nameof(Domain.Constants.MemoryTypes.POSTGRE)
                    }
                );
        }
    }
}
