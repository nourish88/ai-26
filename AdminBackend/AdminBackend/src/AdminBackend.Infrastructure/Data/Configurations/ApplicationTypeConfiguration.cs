using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ApplicationTypeConfiguration : IEntityTypeConfiguration<ApplicationType>
    {
        public void Configure(EntityTypeBuilder<ApplicationType> builder)
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
            builder.HasMany(x => x.Applications).WithOne(y => y.ApplicationType).HasForeignKey(x => x.ApplicationTypeId);

            builder.HasData(
                    new ApplicationType()
                    {
                        Id = Domain.Constants.ApplicationTypes.REACT,
                        Identifier = nameof(Domain.Constants.ApplicationTypes.REACT)
                    },
                    new ApplicationType()
                    {
                        Id = Domain.Constants.ApplicationTypes.CHATBOT,
                        Identifier = nameof(Domain.Constants.ApplicationTypes.CHATBOT)
                    },
                    new ApplicationType()
                    {
                        Id = Domain.Constants.ApplicationTypes.AGENTIC_RAG,
                        Identifier = nameof(Domain.Constants.ApplicationTypes.AGENTIC_RAG)
                    },
                    new ApplicationType()
                    {
                        Id = Domain.Constants.ApplicationTypes.REFLECTIVE_RAG,
                        Identifier = nameof(Domain.Constants.ApplicationTypes.REFLECTIVE_RAG)
                    },
                    new ApplicationType()
                    {
                        Id = Domain.Constants.ApplicationTypes.MCP_POWERED_AGENTIC_RAG,
                        Identifier = nameof(Domain.Constants.ApplicationTypes.MCP_POWERED_AGENTIC_RAG)
                    },
                    new ApplicationType()
                    {
                        Id = Domain.Constants.ApplicationTypes.CUSTOM,
                        Identifier = nameof(Domain.Constants.ApplicationTypes.CUSTOM)
                    }
                );
        }
    }
}
