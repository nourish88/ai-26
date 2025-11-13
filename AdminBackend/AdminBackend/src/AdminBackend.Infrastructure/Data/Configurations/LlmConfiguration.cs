using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class LlmConfiguration : IEntityTypeConfiguration<Llm>
    {
        public void Configure(EntityTypeBuilder<Llm> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.LlmProviderId).IsRequired();
            builder.Property(x => x.Url).IsRequired().HasMaxLength(255);
            builder.Property(x => x.ModelName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.MaxInputTokenSize).IsRequired();
            builder.Property(x => x.MaxOutputTokenSize).IsRequired();
            builder.HasOne(x => x.LlmProvider);

            builder.HasData(
                new Llm()
                {
                    Id = 1,
                    LlmProviderId = 1,
                    MaxInputTokenSize = 32768,
                    MaxOutputTokenSize = 16000,
                    ModelName = "ollama/qwen3:14b",
                    Url = "http://127.0.0.1:4000"
                },
                new Llm()
                {
                    Id = 2,
                    LlmProviderId = 1,
                    MaxInputTokenSize = 32000,
                    MaxOutputTokenSize = 16000,
                    ModelName = "ollama/qwen3:30b",
                    Url = "http://127.0.0.1:4000"
                },
                new Llm()
                {
                    Id = 3,
                    LlmProviderId = 1,
                    MaxInputTokenSize = 32000,
                    MaxOutputTokenSize = 16000,
                    ModelName = "ollama/qwen3:32b",
                    Url = "http://127.0.0.1:4000"
                },
                new Llm()
                {
                    Id = 4,
                    LlmProviderId = 1,
                    MaxInputTokenSize = 32000,
                    MaxOutputTokenSize = 16000,
                    ModelName = "ollama/deepseek-r1:32b",
                    Url = "http://127.0.0.1:4000"
                },
                 new Llm()
                 {
                     Id = 5,
                     LlmProviderId = 1,
                     MaxInputTokenSize = 32000,
                     MaxOutputTokenSize = 16000,
                     ModelName = "remote-ollama/qwen3:14b",
                     Url = "http://127.0.0.1:4000"
                 },
                 new Llm()
                 {
                     Id = 6,
                     LlmProviderId = 1,
                     MaxInputTokenSize = 32000,
                     MaxOutputTokenSize = 16000,
                     ModelName = "remote-ollama/qwen3:32b",
                     Url = "http://127.0.0.1:4000"
                 }
                 ,
                 new Llm()
                 {
                     Id = 7,
                     LlmProviderId = 1,
                     MaxInputTokenSize = 32000,
                     MaxOutputTokenSize = 16000,
                     ModelName = "remote-ollama/gpt-oss:20b",
                     Url = "http://127.0.0.1:4000"
                 },
                 new Llm()
                 {
                     Id = 8,
                     LlmProviderId = 1,
                     MaxInputTokenSize = 32000,
                     MaxOutputTokenSize = 16000,
                     ModelName = "ollama/gpt-oss:20b",
                     Url = "http://127.0.0.1:4000"
                 },
                 new Llm()
                 {
                     Id = 9,
                     LlmProviderId = 1,
                     MaxInputTokenSize = 32000,
                     MaxOutputTokenSize = 16000,
                     ModelName = "hosted_vllm/Qwen/Qwen3-14B",
                     Url = "http://127.0.0.1:4000"
                 },
                 new Llm()
                 {
                     Id = 10,
                     LlmProviderId = 1,
                     MaxInputTokenSize = 32000,
                     MaxOutputTokenSize = 16000,
                     ModelName = "hosted_vllm/Qwen/Qwen3-14B-AWQ",
                     Url = "http://127.0.0.1:4000"
                 }
                );

        }
    }
}
