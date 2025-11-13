namespace Juga.Abstractions.TaskScheduling
{
    [Serializable]
    public class RecurringJobStateResult
    {
        /// <summary>
        /// Belirtilen tekarlanan işin bulunup bulunmadığını açıklayan basit mesaj.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Tekrarlanan iş Unique identifier bilgisi.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Tekrarlanan iş cron expression.
        /// </summary>
        public string Cron { get; set; }

        /// <summary>
        /// Cron expression okunabilir formatı.
        /// </summary>
        public string DescribedCron { get; set; }

        /// <summary>
        /// Parse edilmiş cron expression ı belirten CronConfiguration nesnesi.
        /// </summary>
        public CronConfiguration ParsedCron { get; set; }

        /// <summary>
        /// Tekrarlanan işin Time zone bilgisi.
        /// </summary>
        public string TimeZone { get; set; }

        /// <summary>
        /// Tekrarlanan işin çalışacağı kuyruk bilgisi.
        /// </summary>
        public string Queue { get; set; }

        /// <summary>
        /// Tekrarlanan iş olarak tanımlanmış methodun class bilgisi.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Tekrarlanan iş olarak tanımlanmış methodun method bilgisi.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Tekrarlanan işin oluşturulma zamanı.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Tekrarlanan işin son çalışma zamanı.
        /// </summary>
        public DateTime? LastExecution { get; set; }

        /// <summary>
        /// Tekrarlanan işin son çalışmasının durum bilgisi.
        /// </summary>
        public string LastExecutionState { get; set; }

        /// <summary>
        /// Tekrarlanan işin son çalımasını ifade eden Unique identifier.
        /// </summary>
        public string LastExecutionId { get; set; }

        /// <summary>
        /// Tekrarlanan işin bir sonraki çalışma zamanı.
        /// </summary>
        public DateTime? NextExecution { get; set; }
    }
}