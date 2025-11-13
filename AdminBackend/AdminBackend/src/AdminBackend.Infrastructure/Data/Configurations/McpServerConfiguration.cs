using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class McpServerConfiguration : IEntityTypeConfiguration<McpServer>
    {
        public void Configure(EntityTypeBuilder<McpServer> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x=>x.CreatedDate).IsRequired(false);
            builder.Property(x=>x.CreatedBy).IsRequired(false);
            builder.Property(x=>x.CreatedAt).IsRequired(false);
            builder.Property(x => x.Identifier).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.Identifier).IsUnique();
            builder.Property(x => x.Uri).IsRequired().HasMaxLength(255);
            builder.HasIndex(x => x.Uri).IsUnique();
            builder.HasMany(x => x.ApplicationMcpServers).WithOne(y => y.McpServer).HasForeignKey(x => x.McpServerId);
            builder.HasData(
                    new McpServer()
                    {
                        Id = 1,
                        Identifier = "toolgateway",
                        Uri = "http://localhost:5164/api/mcp"
                    }
                );
        }
    }
}
