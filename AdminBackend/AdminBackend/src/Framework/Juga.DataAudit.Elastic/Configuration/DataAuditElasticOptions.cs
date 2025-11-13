namespace Juga.DataAudit.Elastic.Configuration;

public class DataAuditElasticOptions
{
    /// <summary>
    /// DataAuditElasticOptions ın konfigurasyonda bulunduğu section bilgisi. 
    /// </summary>
    /// 
    public const string DataAuditElasticOptionsSection = "Juga:DataAudit:Elastic";
    /// <summary>
    /// ElasticSearch hostlarını belirtir.
    /// </summary>
    public virtual string Hosts { get; set; }

    /// <summary>
    /// ElasticSearch request timeoutunu(sn) belirtmek için kullanılır.
    /// </summary>
    public virtual int Timeout { get; set; }

    /// <summary>
    /// Audit Logların saklanacağı index in ismini belirtmek için kullanılır.
    /// </summary>
    public virtual string IndexName { get; set; }

    /// <summary>
    /// Basic authentication yöntemi için kullanılan username bilgisi.
    /// </summary>
    public virtual string UserName { get; set; }

    /// <summary>
    /// Basic authentication yöntemi için kullanılan password bilgisi.
    /// </summary>
    public virtual string Password { get; set; }
}