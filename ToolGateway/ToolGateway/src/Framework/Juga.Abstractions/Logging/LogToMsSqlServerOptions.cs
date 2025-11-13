namespace Juga.Abstractions.Logging
{
    /// <summary>
    /// MSSQL Server a loglama ayarları.
    /// </summary>
    public class LogToMsSqlServerOptions
    {
        /// <summary>
        /// Connection string bilgisi.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Log tablosu yoksa otoamtik oluşturulması için. Default false.
        /// </summary>
        public bool AutoCreateSqlTable { get; set; } = false;

        /// <summary>
        /// Log tablosunun şeması. Default dbo.
        /// </summary>
        public string SchemaName { get; set; } = "dbo";

        /// <summary>
        /// Log tablosunun adı. Default Logs.
        /// </summary>
        public string TableName { get; set; } = "Logs";

        /// <summary>
        /// Logların veritabanına kaydı için geçebilecek max sn.Default 5sn
        /// </summary>
        public int BatchPeriod { get; set; } = 5;

        /// <summary>
        /// Logları veritabanına yazmadan önce minumum ne kadar kayıt adedi olması gerektiği. Default 50.
        /// </summary>
        public int BatchPostLimit { get; set; } = 50;

        /// <summary>
        /// Log kayıtlarının oluşturulacağı minimum log seviyesi.Default Information.
        /// </summary>
        public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
    }
}