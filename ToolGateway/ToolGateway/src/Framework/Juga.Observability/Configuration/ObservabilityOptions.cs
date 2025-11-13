namespace Juga.Observability.Configuration
{
    /// <summary>
    /// Observability konfigurasyonu için kullanılır
    /// </summary>
    public class ObservabilityOptions
    {
        /// <summary>
        /// ObservabilityOptions ın konfigurasyonda bulunduğu section bilgisi. 
        /// </summary>
        /// 
        public const string ObservabilityOptionsSection = "Juga:Observability";

        public bool Enabled { get; set; }

        /// <summary>
        /// Log kayıtlarında gösterilecek servis adını belirlemek için kullanılır.
        /// </summary>
        public string ServiceName { get; set; } = default!;

        /// <summary>
        /// Log kayıtlarında gösterilecek servis versiyon bilgisini belirlemek için kullanılır.
        /// </summary>
        public string ServiceVersion { get; set; } = default!;

        /// <summary>
        /// Open telemetry collector url adresini belirlemek için kullanılır.
        /// </summary>
        public string CollectorUrl { get; set; } = @"http://localhost:4317";

        /// <summary>
        /// Tracing işleminin aktif olup olmadığını belirlemek için kullanılır.
        /// </summary>
        public bool EnabledTracing { get; set; } = false;

        /// <summary>
        /// Metric bilgilerinin toplanıp toplanmayacağını belirlemek için kullanılır.
        /// </summary>
        public bool EnabledMetrics { get; set; } = false;

        internal Uri CollectorUri => new(this.CollectorUrl);

        internal string OtlpLogsCollectorUrl => $"{this.CollectorUrl}/v1/logs";
    }
}
