namespace Juga.Abstractions.Logging
{
    public class LogToElasticSearchOptions
    {
        /// <summary>
        /// Log kayıtlarının oluşturulacağı minimum log seviyesi.
        /// </summary>
        public LogLevel MinimumLevel { get; set; }

        /// <summary>
        /// ElasticSearch sunucusunun adresi.
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// ElasticSearch de otomatik olarak log indexleri için index template oluşturulup oluşturulmacağı bilgisi. Default false.
        /// </summary>
        public bool AutoRegisterTemplate { get; set; } = false;

        /// <summary>
        /// ElasticSearch de otomatik olarak oluşturulacak index template inin adı.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Log kayıtlarının kayıt altına alınacağı index in isimlendirilme formatı.
        /// </summary>
        public string IndexFormat { get; set; }
    }
}