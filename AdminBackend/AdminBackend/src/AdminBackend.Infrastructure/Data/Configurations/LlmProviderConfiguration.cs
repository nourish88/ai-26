using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class LlmProviderConfiguration : IEntityTypeConfiguration<LlmProvider>
    {
        public void Configure(EntityTypeBuilder<LlmProvider> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.CreatedDate).IsRequired(false);
            builder.Property(x=>x.CreatedBy).IsRequired(false);
            builder.Property(x=>x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasMany(x => x.Llms).WithOne(y => y.LlmProvider).HasForeignKey(x => x.LlmProviderId);
            builder.HasMany(x => x.Embeddings).WithOne(y => y.LlmProvider).HasForeignKey(x => x.LlmProviderId);
            builder.HasData(
                    new LlmProvider()
                    {
                        Id = (long)LlmProviderTypes.LiteLlm,
                        Name = LlmProviderNames.LITE_LLM
                    }
                );
        }
    }
}
