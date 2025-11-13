using Juga.Abstractions.Data.AuditLog;
using Juga.DataAudit.Elastic.Configuration;
using Microsoft.Extensions.Options;
using Nest;

namespace Juga.DataAudit.Elastic;

public class AuditLogStoreElastic(IOptions<DataAuditElasticOptions> dataAuditElasticOptions,
        IElasticClientProvider elasticClientProvider)
    : IAuditLogStore
{
    private readonly DataAuditElasticOptions _dataAuditElasticOptions = dataAuditElasticOptions.Value;
    private readonly ElasticClient elasticClient = elasticClientProvider.Client;

    public void StoreAuditEvents(IEnumerable<AuditEvent> auditEventsFucn)
    {
        StoreAuditEventsInternal(auditEventsFucn);
    }

    private void StoreAuditEventsInternal(IEnumerable<AuditEvent> auditEvents)
    {
        var response = elasticClient.IndexMany<AuditEvent>(auditEvents);
        if (!response.IsValid)
        {
            throw new Exception($"AuditLog Elastic Error {response.ItemsWithErrors.FirstOrDefault()?.Error?.Reason}");
        }
    }
}