using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    internal class ApplicationExtractorEngineConfiguration : IEntityTypeConfiguration<ApplicationExtractorEngine>
    {
        public void Configure(EntityTypeBuilder<ApplicationExtractorEngine> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.HasOne(x => x.Application);
            builder.HasOne(x => x.ExtractorEngineType);
            
            builder.HasData(
                new ApplicationExtractorEngine()
                {
                    Id = 1,
                    ApplicationId = 1,
                    ExtractorEngineTypeId = 1,
                }
            );
        }
    }
}
