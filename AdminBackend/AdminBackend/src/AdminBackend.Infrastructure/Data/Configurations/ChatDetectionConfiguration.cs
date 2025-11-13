using AdminBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminBackend.Infrastructure.Data.Configurations
{
    public class ChatDetectionConfiguration : IEntityTypeConfiguration<ChatDetection>
    {
        public void Configure(EntityTypeBuilder<ChatDetection> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CreatedDate).IsRequired(false);
            builder.Property(x => x.CreatedBy).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired(false);
            builder.Property(x => x.ApplicationIdentifier).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ThreadId).IsRequired().HasMaxLength(50);
            builder.Property(x => x.MessageId).IsRequired().HasMaxLength(50);
            builder.Property(x => x.UserMessage).IsRequired();
            builder.Property(x => x.Sources).IsRequired(false);
            builder.Property(x => x.Reason).IsRequired(false);
        }
    }
}
