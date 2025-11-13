using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class SearchEngineConfiguration : IEntityTypeConfiguration<SearchEngine>
    {
        public void Configure(EntityTypeBuilder<SearchEngine> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Url).IsRequired().HasMaxLength(255);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.HasOne(x => x.SearchEngineType);
            builder.HasMany(x => x.ApplicationSearchEngines).WithOne(y => y.SearchEngine).HasForeignKey(x => x.SearchEngineId);

            builder.HasData(
                    new SearchEngine()
                    {
                        Id = 1,
                        Identifier="elastic-local",
                        SearchEngineTypeId = (long)SearchEngineTypes.Elastic,
                        Url="http://127.0.0.1:9200"
                    }
                );
        }
    }
}
