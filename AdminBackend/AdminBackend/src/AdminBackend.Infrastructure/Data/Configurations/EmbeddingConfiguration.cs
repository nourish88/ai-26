using AdminBackend.Domain.Constants;
using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class EmbeddingConfiguration : IEntityTypeConfiguration<Embedding>
    {
        public void Configure(EntityTypeBuilder<Embedding> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.LlmProviderId).IsRequired();
            builder.Property(x => x.Url).IsRequired().HasMaxLength(255);
            builder.Property(x => x.ModelName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.VectorSize).IsRequired();
            builder.Property(x => x.MaxInputTokenSize).IsRequired();
            builder.HasOne(x => x.LlmProvider);
            builder.HasMany(x => x.ApplicationSearchEngines).WithOne(y => y.Embedding).HasForeignKey(x => x.EmbeddingId);

            builder.HasData(
                new Embedding()
                {
                    Id = 1,
                    LlmProviderId = (long)LlmProviderTypes.LiteLlm,
                    Url = "http://127.0.0.1:4000",
                    ModelName = "lm_studio/text-embedding-qwen3-embedding-4b",
                    VectorSize = 2560,
                    MaxInputTokenSize = 32768
                },
                new Embedding()
                {
                    Id = 2,
                    LlmProviderId = (long)LlmProviderTypes.LiteLlm,
                    Url = "http://127.0.0.1:4000",
                    ModelName = "ollama/dengcao/Qwen3-Embedding-8B:Q8_0",
                    VectorSize = 2560,
                    MaxInputTokenSize = 32768
                },
                new Embedding()
                {
                    Id = 3,
                    LlmProviderId = (long)LlmProviderTypes.LiteLlm,
                    Url = "http://127.0.0.1:4000",
                    ModelName = "remote-ollama/dengcao/Qwen3-Embedding-4B:Q8_0",
                    VectorSize = 2560,
                    MaxInputTokenSize = 32768
                }

            );
        }
    }
}
