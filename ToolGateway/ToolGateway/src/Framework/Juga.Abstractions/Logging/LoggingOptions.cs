namespace Juga.Abstractions.Logging
{
    /// <summary>
    /// appsettings.json dosyasında tanımlanan logging konfigurasyonu modelidir.
    /// EnableWriteToFile: dosyaya loglama özelliğini açar
    /// LogToFileOptions: dosya logu konfigurasyonu
    /// EnableWriteToElasticSearch: elastic loglamayı açar
    /// LogToElasticSearchOptions: elastic konfigurasyonu
    /// </summary>
    public class LoggingOptions
    {
        public const string OptionsSection = "Juga:Logging";

        /// <summary>
        /// Log kayıtlarında bulunacak uygulama adı bilgisi.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Uygulama seviyesinde kullanılacak log provider.
        /// </summary>
        public LoggerType LoggerType { get; set; }

        /// <summary>
        /// Log provider ın kendi loglarını kayıt altına alıp almayacağı bilgisi.
        /// </summary>
        public bool EnableSelfLog { get; set; }

        /// <summary>
        /// Log providerın log seviyesi ayarlarının microsoftun ayarlarını ezip ezmeyeceği bilgisi.
        /// </summary>
        public bool OverrideMicrosoftLevels { get; set; }

        /// <summary>
        /// Logların consola yazılıp yazılmayacağı bilgisi.
        /// </summary>
        public bool EnableWriteToConsole { get; set; }

        /// <summary>
        /// Logların debug a yazılıp yazılmayacağının bilgisi.
        /// </summary>
        public bool EnableWriteToDebug { get; set; }

        /// <summary>
        /// Logların dosyaya yazılıp yazılmayacağı bilgisi.
        /// </summary>
        public bool EnableWriteToFile { get; set; }

        /// <summary>
        /// Logların dosyaya yazılması durumunda kullanılacak dosyaya yazma konfigurasyonu.
        /// </summary>
        public LogToFileOptions LogToFileOptions { get; set; }

        /// <summary>
        /// Logların ElasticSearch e yazılıp yazılmayacağı bilgisi.
        /// </summary>
        public bool EnableWriteToElasticSearch { get; set; }

        /// <summary>
        /// Logların ElasticSearch e yazılması durumunda kullanılacak ElasticSearch e yazmak için kullanılacak konfigurasyon.
        /// </summary>
        public LogToElasticSearchOptions LogToElasticSearchOptions { get; set; }

        /// <summary>
        /// Default log providerların uygulama seviyesinde geçersiz kılınıp kılınmayacağı bilgisi.
        /// </summary>
        public bool ClearDefaultLogProviders { get; set; }

        /// <summary>
        /// Request response verilerinin loglanıp loglanmayacağı bilgisi.
        /// </summary>
        public bool EnableRequestResponseLogging { get; set; }

        /// <summary>
        /// Logların MsSql Server a yazılıp yazılmayacağı bilgisi.
        /// </summary>
        public bool EnableWriteToMsSqlServer { get; set; }

        /// <summary>
        /// Logların MsSql Server a yazılması durumunda kullanılacak MsSql e yazma konfigurasyonu.
        /// </summary>
        public LogToMsSqlServerOptions LogToMsSqlServerOptions { get; set; }

        /// <summary>
        /// Logların Seq e yazılıp yazılmayacağı bilgisi.
        /// </summary>
        public bool EnableWriteToSeq { get; set; } //

        /// <summary>
        /// Logların Seq a yazılması durumunda kullanılacak konfigurasyon.
        /// </summary>
        public LogToSeqOptions LogToSeqOptions { get; set; }
        /// <summary>
        /// Logların Postgresql a yazılıp yazılmayacağı bilgisi.
        /// </summary>
        public bool EnableWriteToPostgresql { get; set; }
        /// <summary>
        /// Logların Postgresql e yazılması durumunda kullanılacak Postgresql e yazma konfigurasyonu.
        /// </summary>
        public LogToPostgresqlOptions LogToPostgresqlOptions { get; set; }
        /// <summary>
        /// Logların open telemetry e yazılıp yazılmayacağı bilgisi.
        /// </summary>
        public bool EnableWriteToOpenTelemetry { get; set; }
        /// <summary>
        /// Logların open telemetry e yazılması durumunda kullanılacak open telemetry e yazma konfigurasyonu.
        /// </summary>
        public LogToOpenTelemetryOptions LogToOpenTelemetryOptions { get; set; }
        /// <summary>
        /// <summary>
        /// Belirtilen source contextlerin loglarının yazılmaması için kullanılır.
        /// </summary>
        public List<string> ExcludedSourceContext { get; set; } = new();

        /// <summary>
        /// Belirtilen source contextlerin default log levellarını override etmek için kullanılır.
        /// </summary>
        public Dictionary<string, LogLevel> OverrideLevels { get; set; } = new();
    }
}