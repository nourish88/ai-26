
using Nest;

namespace Juga.DataAudit.Elastic;

public interface IElasticClientProvider
{
    ElasticClient Client { get; }
}