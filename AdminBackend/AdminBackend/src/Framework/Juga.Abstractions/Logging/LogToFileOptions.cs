namespace Juga.Abstractions.Logging
{
    public class LogToFileOptions
    {
        /// <summary>
        /// Log dosyalarının oluşturulacağı dosya yolu.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Log kayıtlarının oluşturulacağı minimum log seviyesi.
        /// </summary>
        public LogLevel MinimumLevel { get; set; }

        /// <summary>
        /// Log dosyalarının erişebileceği maximum dosya büyüklüğü.
        /// </summary>
        public long? FileSizeLimitBytes { get; set; }

        /// <summary>
        /// Yeni log dosyalarının hangi aralıklarla oluşturulacağı. Defaul Day.
        /// </summary>
        public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
    }
}