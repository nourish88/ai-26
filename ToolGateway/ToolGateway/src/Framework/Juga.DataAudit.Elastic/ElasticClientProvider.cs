using Elasticsearch.Net;
using Juga.DataAudit.Elastic.Configuration;
using Microsoft.Extensions.Options;
using Nest;

namespace Juga.DataAudit.Elastic;

internal class ElasticClientProvider : IElasticClientProvider
{
    private readonly DataAuditElasticOptions _dataAuditElasticOptions;

    public ElasticClientProvider(IOptions<DataAuditElasticOptions> dataAuditElasticOptions)
    {
        this._dataAuditElasticOptions = dataAuditElasticOptions.Value;
        Client = CreateClient();
    }
    public ElasticClient Client { get; }

    private ElasticClient CreateClient()
    {
        var connectionPool = new SingleNodeConnectionPool(new Uri(_dataAuditElasticOptions.Hosts));
        var connectionSettings = new ConnectionSettings(connectionPool)
            .ThrowExceptions(true)
            .RequestTimeout(TimeSpan.FromSeconds(_dataAuditElasticOptions.Timeout))
            .DefaultMappingFor<Juga.Abstractions.Data.AuditLog.AuditEvent>(m => m.IndexName(_dataAuditElasticOptions.IndexName))
            .EnableApiVersioningHeader();

        if (!(string.IsNullOrEmpty(_dataAuditElasticOptions.UserName)|| string.IsNullOrEmpty(_dataAuditElasticOptions.Password)))
        {
            connectionSettings.BasicAuthentication(_dataAuditElasticOptions.UserName, _dataAuditElasticOptions.Password);
        }

        return new ElasticClient(connectionSettings);
    }
}