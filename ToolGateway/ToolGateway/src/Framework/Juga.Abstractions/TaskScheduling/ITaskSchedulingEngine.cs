using System.Linq.Expressions;

namespace Juga.Abstractions.TaskScheduling
{
    public interface ITaskSchedulingEngine
    {
        /// <summary>
        /// Belirtilen expression a göre fire and forget job oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
        string Enqueue(Expression<Action> methodCall);

        /// <summary>
        /// Belirtilen expression a göre fire and forget job oluşturmak için kullanılır.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="methodCall"></param>
        /// <returns>>Oluşturulan job için Unique identifier bilgisi.</returns>
        string Enqueue<TContract>(Expression<Action<TContract>> methodCall);

        /// <summary>
        /// Belirtilen expression ve gecikme süresine e göre fire and forget job oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="delay"></param>
        /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
        string Schedule(Expression<Action> methodCall, TimeSpan delay);

        /// <summary>
        /// Belirtilen expression ve gecikme süresine e göre fire and forget job oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="delay"></param>
        /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
        string Schedule<TContract>(Expression<Action<TContract>> methodCall, TimeSpan delay);

        /// <summary>
        /// Belirtilen expression ve çalışma zamanına e göre fire and forget job oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="delay"></param>
        /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
        string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt);

        /// <summary>
        /// Belirtilen expression ve çalışma zamanına e göre fire and forget job oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall"></param>
        /// <param name="delay"></param>
        /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
        string Schedule<TContract>(Expression<Action<TContract>> methodCall, DateTimeOffset enqueueAt);

        /// <summary>
        /// Belirilen background job ı silmek için kullanılır.
        /// </summary>
        /// <param name="jobId">Background job Unique identifier.</param>
        bool DeleteJob(string jobId);

        /// <summary>
        /// Belirtilen cron expression a göre tekrarlanan joblar oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="cronExpression">Tekrarlanan iş için cron expression.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
        string AddRecurringJob(Expression<Action> methodCall, string cronExpression, string queueName = "default");

        /// <summary>
        /// Belirtilen cron expression a göre tekrarlanan joblar oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="cronExpression">Tekrarlanan iş için cron expression.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
        string AddRecurringJob<TContract>(Expression<Action<TContract>> methodCall, string cronExpression, string queueName = "default");

        /// <summary>
        /// Tanımlanmış bir tekrarlanan işin güncellenmesi için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="cronExpression">Tekrarlanan iş için cron expression.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void UpdateRecurringJob(Expression<Action> methodCall, string cronExpression, string recurringJobId, string queueName = "default");

        /// <summary>
        /// Tanımlanmış bir tekrarlanan işin güncellenmesi için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="cronExpression">Tekrarlanan iş için cron expression.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void UpdateRecurringJob<TContract>(Expression<Action<TContract>> methodCall, string cronExpression, string recurringJobId, string queueName = "default");

        /// <summary>
        /// Belirtilen dakika aralığında tekrarlanan iş oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="intervalInMinutes">İşin kaç dakikada bir tekrarlanacağı.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void RecurringJobMinutely(Expression<Action> methodCall, int intervalInMinutes, string recurringJobId = null, string queueName = "default");

        /// <summary>
        /// Belirtilen dakika aralığında tekrarlanan iş oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="intervalInMinutes">İşin kaç dakikada bir tekrarlanacağı.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void RecurringJobMinutely<TContract>(Expression<Action<TContract>> methodCall, int intervalInMinutes, string recurringJobId = null, string queueName = "default");

        /// <summary>
        /// Belirtilen saat ve dakikada her gün tekrarlanan iş oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
        /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void RecurringJobDaily(Expression<Action> methodCall, int hour, int min, string recurringJobId = null, string queueName = "default");

        /// <summary>
        /// Belirtilen saat ve dakikada her gün tekrarlanan iş oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
        /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void RecurringJobDaily<TContract>(Expression<Action<TContract>> methodCall, int hour, int min, string recurringJobId = null, string queueName = "default");

        /// <summary>
        ///  Haftanın belirtilen günlerinde,belirtilen saat ve dakikada tekrarlanan iş oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="days">İşin tekrarlanacağı günlerin bilgisi.</param>
        /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
        /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void RecurringJobWeekly(Expression<Action> methodCall, List<CronExpressionDay> days, int hour, int min, string recurringJobId = null, string queueName = "default");

        /// <summary>
        ///  Haftanın belirtilen günlerinde,belirtilen saat ve dakikada tekrarlanan iş oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="days">İşin tekrarlanacağı günlerin bilgisi.</param>
        /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
        /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void RecurringJobWeekly<TContract>(Expression<Action<TContract>> methodCall, List<CronExpressionDay> days, int hour, int min, string recurringJobId = null, string queueName = "default");

        /// <summary>
        /// Ayın belirtilen günlerinde,belirtilen saat ve dakikada tekrarlanan iş oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="daysOfMonth">İşin tekrarlanacağı günlerin bilgisi.</param>
        /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
        /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void RecurringJobMonthly(Expression<Action> methodCall, List<int> daysOfMonth, int hour, int min, string recurringJobId = null, string queueName = "default");

        /// <summary>
        /// Ayın belirtilen günlerinde,belirtilen saat ve dakikada tekrarlanan iş oluşturmak için kullanılır.
        /// </summary>
        /// <param name="methodCall">Tekrarlanacak iş.</param>
        /// <param name="daysOfMonth">İşin tekrarlanacağı günlerin bilgisi.</param>
        /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
        /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
        void RecurringJobMonthly<TContract>(Expression<Action<TContract>> methodCall, List<int> daysOfMonth, int hour, int min, string recurringJobId = null, string queueName = "default");

        /// <summary>
        /// Belirtilen tekrarlanan işin silinmesi için kullanılır.
        /// </summary>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        void RemoveRecurringJobWithId(string recurringJobId);

        /// <summary>
        /// Belirtilen tekrarlanan işin manual tetiklenmesi için kullanılır.
        /// </summary>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        void TriggerRecurringJobWithId(string recurringJobId);

        /// <summary>
        /// Belirtilen tekrarlanan işin durum bilgisini almak için kullanılır.
        /// </summary>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <returns>Tekrarlanan iş durum bilgisi.</returns>
        RecurringJobStateResult GetRecurringJobState(string recurringJobId);

        /// <summary>
        /// Tüm tekrarlanan işlerin kaldırılması için kullanılır.
        /// </summary>
        void RemoveAllRecurringJobs();

        /// <summary>
        /// Belirtilen tekrarlanan işin olup olmadığını belirlemek için kullanılır.
        /// </summary>
        /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
        /// <returns>Belirtielen tekrarlanan işin olup olmadığı bilgisi.</returns>
        bool RecurringJobExists(string recurringJobId, string queueName = "default");

        /// <summary>
        /// Cron expression ı okunabilir bir formata dönüştürmek için kullanılır.
        /// </summary>
        /// <param name="cron">Cron expression.</param>
        /// <returns>Cron expression ın okunabilir hali.</returns>
        string DescribeCron(string cron);

        CronConfiguration ParseCron(string cron);

        void Dispose();
    }
}