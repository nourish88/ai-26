using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ApplicationLlmConfiguration : IEntityTypeConfiguration<ApplicationLlm>
    {
        public void Configure(EntityTypeBuilder<ApplicationLlm> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.TopP).IsRequired(true);
            builder.Property(x => x.Temperature).IsRequired(true);
            builder.Property(x => x.EnableThinking).IsRequired(true);
            builder.HasOne(x => x.Llm);
            builder.HasOne(x => x.Application);
            builder.HasIndex(x => new { x.ApplicationId, x.LlmId }).IsUnique();

            builder.HasData(
                new ApplicationLlm()
                {
                    Id = 1,
                    ApplicationId = 1,
                    LlmId = 6,
                    EnableThinking = false,
                    Temperature = 0,
                    TopP = 0.7F,
                }                
            );
        }
    }
}
