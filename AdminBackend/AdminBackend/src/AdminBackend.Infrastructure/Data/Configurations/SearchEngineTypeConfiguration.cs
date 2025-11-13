using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class SearchEngineTypeConfiguration : IEntityTypeConfiguration<SearchEngineType>
    {
        public void Configure(EntityTypeBuilder<SearchEngineType> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.HasMany(x => x.SearchEngines).WithOne(y => y.SearchEngineType).HasForeignKey(x => x.SearchEngineTypeId);

            builder.HasData(
                new SearchEngineType()
                {
                    Id = (long)SearchEngineTypes.Elastic,
                    Identifier= SearchEngineTypeNames.ELASTICSEARCH
                }    
            );
        }
    }
}
