
using Juga.MessageQueue.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Juga.MessageQueue.Extensions;

public static class MigrationHelper
{
    public static void SetOutboxMigration(this ModelBuilder modelBuilder,IConfiguration configuration)
    {


        var queueConfigSection = configuration.GetSection("Juga:Queue");
        var isQueueEnabled = queueConfigSection.GetValue<bool>("IsEnabled");
        var queueType = queueConfigSection.GetValue<QueueType>("QueueType");
        var isQueueSetEnabledAndSet = queueConfigSection != null && isQueueEnabled && queueType != QueueType.None;
        if (isQueueSetEnabledAndSet)
        {
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

        }
       
    }
}