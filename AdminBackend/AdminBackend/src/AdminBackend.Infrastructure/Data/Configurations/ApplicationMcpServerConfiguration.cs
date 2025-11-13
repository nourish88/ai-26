using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ApplicationMcpServerConfiguration : IEntityTypeConfiguration<ApplicationMcpServer>
    {
        public void Configure(EntityTypeBuilder<ApplicationMcpServer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.HasOne(x => x.McpServer);
            builder.HasOne(x => x.Application);
            builder.HasIndex(x => new { x.ApplicationId, x.McpServerId }).IsUnique();

            builder.HasData(
                new ApplicationMcpServer()
                {
                    Id = 1,
                    ApplicationId = 1,
                    McpServerId = 1,
                }                
            );
        }
    }
}
