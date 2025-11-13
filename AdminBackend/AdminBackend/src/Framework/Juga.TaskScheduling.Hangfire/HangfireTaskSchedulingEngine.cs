using System.Linq.Expressions;
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.Storage;
using Juga.Abstractions.TaskScheduling;
using Juga.TaskScheduling.Hangfire.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Juga.TaskScheduling.Hangfire;

public class HangfireTaskSchedulingEngine : ITaskSchedulingEngine
{
    private readonly BackgroundJobServer _backgroundJobServer;
    private readonly TaskSchedulingOptions _options;
    private readonly IServiceScopeFactory serviceScopeFactory;

    public HangfireTaskSchedulingEngine(IOptions<TaskSchedulingOptions> options,
        IServiceScopeFactory serviceScopeFactory)
    {
        _options = options.Value;
        this.serviceScopeFactory = serviceScopeFactory;
        if (_options.Enabled)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage(_options.DataBaseName);
            GlobalConfiguration.Configuration.UseActivator(new AspNetCoreJobActivator(this.serviceScopeFactory));
            if (_options.SelfBackgroundJobServer)
            {
                var backgroundJobServerOptions = new BackgroundJobServerOptions
                {
                    Queues = _options.Queues,
                    ServerName = _options.ServerName
                };
                _backgroundJobServer = new BackgroundJobServer(backgroundJobServerOptions);
            }
        }
    }

    public void Dispose()
    {
        if (_backgroundJobServer != null)
        {
            _backgroundJobServer.WaitForShutdown(TimeSpan.FromSeconds(_options.ShutDownTimeout));
            _backgroundJobServer.Dispose();
        }
    }

    /// <summary>
    ///     Belirtilen expression a göre fire and forget job oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall"></param>
    /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
    public string Enqueue(Expression<Action> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    /// <summary>
    ///     Belirtilen expression a göre fire and forget job oluşturmak için kullanılır.
    /// </summary>
    /// <typeparam name="TContract"></typeparam>
    /// <param name="methodCall"></param>
    /// <returns>>Oluşturulan job için Unique identifier bilgisi.</returns>
    public string Enqueue<TContract>(Expression<Action<TContract>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    /// <summary>
    ///     Belirtilen expression ve gecikme süresine e göre fire and forget job oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall"></param>
    /// <param name="delay"></param>
    /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
    public string Schedule(Expression<Action> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    /// <summary>
    ///     Belirtilen expression ve gecikme süresine e göre fire and forget job oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall"></param>
    /// <param name="delay"></param>
    /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
    public string Schedule<TContract>(Expression<Action<TContract>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    /// <summary>
    ///     Belirtilen expression ve çalışma zamanına e göre fire and forget job oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall"></param>
    /// <param name="delay"></param>
    /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
    public string Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    /// <summary>
    ///     Belirtilen expression ve çalışma zamanına e göre fire and forget job oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall"></param>
    /// <param name="delay"></param>
    /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
    public string Schedule<TContract>(Expression<Action<TContract>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    /// <summary>
    ///     Belirilen background job ı silmek için kullanılır.
    /// </summary>
    /// <param name="jobId">Background job Unique identifier.</param>
    /// <returns></returns>
    public bool DeleteJob(string jobId)
    {
        return BackgroundJob.Delete(jobId);
    }

    /// <summary>
    ///     Belirtilen cron expression a göre tekrarlanan joblar oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="cronExpression">Tekrarlanan iş için cron expression.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
    public string AddRecurringJob(Expression<Action> methodCall, string cronExpression, string queueName = "default")
    {
        try
        {
            var methodCallExpression = (MethodCallExpression)methodCall.Body;

            var generatedRecurringJobId = string.Format("{0}.{1}_{2}",
                methodCallExpression.Method.ReflectedType.Name,
                methodCallExpression.Method.Name,
                Guid.NewGuid().ToString());

            RecurringJob.AddOrUpdate(generatedRecurringJobId, methodCall, cronExpression, TimeZoneInfo.Local,
                queueName);

            return generatedRecurringJobId;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    ///     Belirtilen cron expression a göre tekrarlanan joblar oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="cronExpression">Tekrarlanan iş için cron expression.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    /// <returns>Oluşturulan job için Unique identifier bilgisi.</returns>
    public string AddRecurringJob<TContract>(Expression<Action<TContract>> methodCall, string cronExpression,
        string queueName = "default")
    {
        try
        {
            var methodCallExpression = (MethodCallExpression)methodCall.Body;

            var generatedRecurringJobId = string.Format("{0}.{1}_{2}",
                methodCallExpression.Method.ReflectedType.Name,
                methodCallExpression.Method.Name,
                Guid.NewGuid().ToString());

            RecurringJob.AddOrUpdate(generatedRecurringJobId, methodCall, cronExpression, TimeZoneInfo.Local,
                queueName);

            return generatedRecurringJobId;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    ///     Tanımlanmış bir tekrarlanan işin güncellenmesi için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="cronExpression">Tekrarlanan iş için cron expression.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void UpdateRecurringJob(Expression<Action> methodCall, string cronExpression, string recurringJobId,
        string queueName = "default")
    {
        if (!string.IsNullOrEmpty(recurringJobId) && RecurringJobExists(recurringJobId))
            RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression, TimeZoneInfo.Local, queueName);
    }

    /// <summary>
    ///     Tanımlanmış bir tekrarlanan işin güncellenmesi için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="cronExpression">Tekrarlanan iş için cron expression.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void UpdateRecurringJob<TContract>(Expression<Action<TContract>> methodCall, string cronExpression,
        string recurringJobId, string queueName = "default")
    {
        if (!string.IsNullOrEmpty(recurringJobId) && RecurringJobExists(recurringJobId))
            RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression, TimeZoneInfo.Local, queueName);
    }

    /// <summary>
    ///     Belirtilen dakika aralığında tekrarlanan iş oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="intervalInMinutes">İşin kaç dakikada bir tekrarlanacağı.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void RecurringJobMinutely(Expression<Action> methodCall, int intervalInMinutes, string recurringJobId = null,
        string queueName = "default")
    {
        if (intervalInMinutes > 0)
        {
            if (string.IsNullOrEmpty(recurringJobId))
                RecurringJob.AddOrUpdate(methodCall,
                    CronMaker.CronWithMinuteInterval(intervalInMinutes),
                    TimeZoneInfo.Local, queueName);
            else
                RecurringJob.AddOrUpdate(recurringJobId,
                    methodCall,
                    CronMaker.CronWithMinuteInterval(intervalInMinutes),
                    TimeZoneInfo.Local, queueName);
        }
        //Logger.Info("Given 'min' parameter value is below 0. Recurring job is not activated.");
    }

    /// <summary>
    ///     Belirtilen dakika aralığında tekrarlanan iş oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="intervalInMinutes">İşin kaç dakikada bir tekrarlanacağı.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void RecurringJobMinutely<TContract>(Expression<Action<TContract>> methodCall, int intervalInMinutes,
        string recurringJobId = null, string queueName = "default")
    {
        if (intervalInMinutes > 0)
        {
            if (string.IsNullOrEmpty(recurringJobId))
                RecurringJob.AddOrUpdate(methodCall,
                    CronMaker.CronWithMinuteInterval(intervalInMinutes),
                    TimeZoneInfo.Local, queueName);
            else
                RecurringJob.AddOrUpdate(recurringJobId,
                    methodCall,
                    CronMaker.CronWithMinuteInterval(intervalInMinutes),
                    TimeZoneInfo.Local, queueName);
        }
        //Logger.Info("Given 'min' parameter value is below 0. Recurring job is not activated.");
    }

    /// <summary>
    ///     Belirtilen saat ve dakikada her gün tekrarlanan iş oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
    /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void RecurringJobDaily(Expression<Action> methodCall, int hour, int min, string recurringJobId = null,
        string queueName = "default")
    {
        hour = TaskSchedulingHelper.NormalizeHour(hour);
        min = TaskSchedulingHelper.NormalizeMinute(min);

        if (string.IsNullOrEmpty(recurringJobId))
            RecurringJob.AddOrUpdate(methodCall,
                CronMaker.CronWithExactTimeWorksDaily(hour, min),
                TimeZoneInfo.Local, queueName);
        else
            RecurringJob.AddOrUpdate(recurringJobId,
                methodCall,
                CronMaker.CronWithExactTimeWorksDaily(hour, min),
                TimeZoneInfo.Local, queueName);
    }

    /// <summary>
    ///     Belirtilen saat ve dakikada her gün tekrarlanan iş oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
    /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void RecurringJobDaily<TContract>(Expression<Action<TContract>> methodCall, int hour, int min,
        string recurringJobId = null, string queueName = "default")
    {
        hour = TaskSchedulingHelper.NormalizeHour(hour);
        min = TaskSchedulingHelper.NormalizeMinute(min);

        if (string.IsNullOrEmpty(recurringJobId))
            RecurringJob.AddOrUpdate(methodCall,
                CronMaker.CronWithExactTimeWorksDaily(hour, min),
                TimeZoneInfo.Local, queueName);
        else
            RecurringJob.AddOrUpdate(recurringJobId,
                methodCall,
                CronMaker.CronWithExactTimeWorksDaily(hour, min),
                TimeZoneInfo.Local, queueName);
    }

    /// <summary>
    ///     Haftanın belirtilen günlerinde,belirtilen saat ve dakikada tekrarlanan iş oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="days">İşin tekrarlanacağı günlerin bilgisi.</param>
    /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
    /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void RecurringJobWeekly(Expression<Action> methodCall, List<CronExpressionDay> days, int hour, int min,
        string recurringJobId = null, string queueName = "default")
    {
        hour = TaskSchedulingHelper.NormalizeHour(hour);
        min = TaskSchedulingHelper.NormalizeMinute(min);

        if (string.IsNullOrEmpty(recurringJobId))
            RecurringJob.AddOrUpdate(methodCall,
                CronMaker.CronWithExactDayTimeWorksWeekly(days, hour, min),
                TimeZoneInfo.Local, queueName);
        else
            RecurringJob.AddOrUpdate(recurringJobId,
                methodCall,
                CronMaker.CronWithExactDayTimeWorksWeekly(days, hour, min),
                TimeZoneInfo.Local, queueName);
    }

    /// <summary>
    ///     Haftanın belirtilen günlerinde,belirtilen saat ve dakikada tekrarlanan iş oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="days">İşin tekrarlanacağı günlerin bilgisi.</param>
    /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
    /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void RecurringJobWeekly<TContract>(Expression<Action<TContract>> methodCall, List<CronExpressionDay> days,
        int hour, int min, string recurringJobId = null, string queueName = "default")
    {
        hour = TaskSchedulingHelper.NormalizeHour(hour);
        min = TaskSchedulingHelper.NormalizeMinute(min);

        if (string.IsNullOrEmpty(recurringJobId))
            RecurringJob.AddOrUpdate(methodCall,
                CronMaker.CronWithExactDayTimeWorksWeekly(days, hour, min),
                TimeZoneInfo.Local, queueName);
        else
            RecurringJob.AddOrUpdate(recurringJobId,
                methodCall,
                CronMaker.CronWithExactDayTimeWorksWeekly(days, hour, min),
                TimeZoneInfo.Local, queueName);
    }

    /// <summary>
    ///     Ayın belirtilen günlerinde,belirtilen saat ve dakikada tekrarlanan iş oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="daysOfMonth">İşin tekrarlanacağı günlerin bilgisi.</param>
    /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
    /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void RecurringJobMonthly(Expression<Action> methodCall, List<int> daysOfMonth, int hour, int min,
        string recurringJobId = null, string queueName = "default")
    {
        var normalizedDaysOfMonth = new List<int>();

        if (daysOfMonth != null)
            for (var i = 0; i < daysOfMonth.Count(); i++)
            {
                var dayOfMonth = TaskSchedulingHelper.NormalizeDayOfMonth(daysOfMonth[i]);
                normalizedDaysOfMonth.Add(dayOfMonth);
            }

        hour = TaskSchedulingHelper.NormalizeHour(hour);
        min = TaskSchedulingHelper.NormalizeMinute(min);

        if (string.IsNullOrEmpty(recurringJobId))
            RecurringJob.AddOrUpdate(methodCall,
                CronMaker.CronWithExactDayTimeWorksMonthly(normalizedDaysOfMonth, hour, min),
                TimeZoneInfo.Local, queueName);
        else
            RecurringJob.AddOrUpdate(recurringJobId,
                methodCall,
                CronMaker.CronWithExactDayTimeWorksMonthly(normalizedDaysOfMonth, hour, min),
                TimeZoneInfo.Local, queueName);
    }

    /// <summary>
    ///     Ayın belirtilen günlerinde,belirtilen saat ve dakikada tekrarlanan iş oluşturmak için kullanılır.
    /// </summary>
    /// <param name="methodCall">Tekrarlanacak iş.</param>
    /// <param name="daysOfMonth">İşin tekrarlanacağı günlerin bilgisi.</param>
    /// <param name="hour">İşin tekrarlanacağı saat bilgisi.</param>
    /// <param name="min">İşin tekrarlanacağı dakika bilgisi.</param>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <param name="queueName">Job kuyruk ismi. Default değer "default".</param>
    public void RecurringJobMonthly<TContract>(Expression<Action<TContract>> methodCall, List<int> daysOfMonth,
        int hour, int min, string recurringJobId = null, string queueName = "default")
    {
        var normalizedDaysOfMonth = new List<int>();

        if (daysOfMonth != null)
            for (var i = 0; i < daysOfMonth.Count(); i++)
            {
                var dayOfMonth = TaskSchedulingHelper.NormalizeDayOfMonth(daysOfMonth[i]);
                normalizedDaysOfMonth.Add(dayOfMonth);
            }

        hour = TaskSchedulingHelper.NormalizeHour(hour);
        min = TaskSchedulingHelper.NormalizeMinute(min);

        if (string.IsNullOrEmpty(recurringJobId))
            RecurringJob.AddOrUpdate(methodCall,
                CronMaker.CronWithExactDayTimeWorksMonthly(normalizedDaysOfMonth, hour, min),
                TimeZoneInfo.Local, queueName);
        else
            RecurringJob.AddOrUpdate(recurringJobId,
                methodCall,
                CronMaker.CronWithExactDayTimeWorksMonthly(normalizedDaysOfMonth, hour, min),
                TimeZoneInfo.Local, queueName);
    }

    /// <summary>
    ///     Belirtilen tekrarlanan işin silinmesi için kullanılır.
    /// </summary>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    public void RemoveRecurringJobWithId(string recurringJobId)
    {
        if (!string.IsNullOrEmpty(recurringJobId)) RecurringJob.RemoveIfExists(recurringJobId);
    }

    /// <summary>
    ///     Belirtilen tekrarlanan işin manual tetiklenmesi için kullanılır.
    /// </summary>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    public void TriggerRecurringJobWithId(string recurringJobId)
    {
        if (!string.IsNullOrEmpty(recurringJobId)) RecurringJob.Trigger(recurringJobId);
    }

    /// <summary>
    ///     Belirtilen tekrarlanan işin durum bilgisini almak için kullanılır.
    /// </summary>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <returns>Tekrarlanan iş durum bilgisi.</returns>
    public RecurringJobStateResult GetRecurringJobState(string recurringJobId)
    {
        var jobStateResult = new RecurringJobStateResult();

        using (var connection = JobStorage.Current.GetConnection())
        {
            var recurringJob = connection.GetRecurringJobs().FirstOrDefault(job => job.Id.Equals(recurringJobId));

            if (recurringJob != null)
            {
                var createdAt = recurringJob.CreatedAt;
                var lastExecution = recurringJob.LastExecution;
                var nextExecution = recurringJob.NextExecution;
                var timezoneId = recurringJob.TimeZoneId;

                if (createdAt != null)
                    recurringJob.CreatedAt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(createdAt.Value, timezoneId);

                if (lastExecution != null)
                    recurringJob.LastExecution =
                        TimeZoneInfo.ConvertTimeBySystemTimeZoneId(lastExecution.Value, timezoneId);

                if (nextExecution != null)
                    recurringJob.NextExecution =
                        TimeZoneInfo.ConvertTimeBySystemTimeZoneId(nextExecution.Value, timezoneId);

                if (recurringJob.Job != null)
                {
                    jobStateResult.Message = "JOB_FOUND";
                    jobStateResult.Id = recurringJob.Id;
                    jobStateResult.Cron = recurringJob.Cron;
                    jobStateResult.DescribedCron = CronDescriptor.DescribeCron(recurringJob.Cron);
                    jobStateResult.ParsedCron = CronDescriptor.ParseCron(recurringJob.Cron);
                    jobStateResult.TimeZone = recurringJob.TimeZoneId;
                    jobStateResult.Queue = recurringJob.Queue;
                    jobStateResult.ClassName = recurringJob.Job?.Method.DeclaringType.FullName;
                    jobStateResult.MethodName = recurringJob.Job?.Method.Name;
                    jobStateResult.CreatedAt = recurringJob.CreatedAt;
                    jobStateResult.LastExecution = recurringJob.LastExecution;
                    jobStateResult.LastExecutionState = recurringJob.LastJobState;
                    jobStateResult.LastExecutionId = recurringJob.LastJobId;
                    jobStateResult.NextExecution = recurringJob.NextExecution;
                }
                else
                {
                    jobStateResult.Message = "JOB_FOUND_METHOD_NOT_FOUND";
                    jobStateResult.Id = recurringJob.Id;
                    jobStateResult.Cron = recurringJob.Cron;
                    jobStateResult.DescribedCron = CronDescriptor.DescribeCron(recurringJob.Cron);
                    jobStateResult.ParsedCron = CronDescriptor.ParseCron(recurringJob.Cron);
                    jobStateResult.TimeZone = recurringJob.TimeZoneId;
                    jobStateResult.Queue = recurringJob.Queue;
                    jobStateResult.ClassName = null;
                    jobStateResult.MethodName = null;
                    jobStateResult.CreatedAt = recurringJob.CreatedAt;
                    jobStateResult.LastExecution = recurringJob.LastExecution;
                    jobStateResult.LastExecutionState = recurringJob.LastJobState;
                    jobStateResult.LastExecutionId = recurringJob.LastJobId;
                    jobStateResult.NextExecution = recurringJob.NextExecution;
                }
            }
            else
            {
                jobStateResult.Message = "JOB_NOT_FOUND";
            }
        }

        return jobStateResult;
    }

    /// <summary>
    ///     Tüm tekrarlanan işlerin kaldırılması için kullanılır.
    /// </summary>
    public void RemoveAllRecurringJobs()
    {
        using (var connection = JobStorage.Current.GetConnection())
        {
            var setKey = "recurring-jobs";
            var savedJobIds = connection.GetAllItemsFromSet(setKey);

            foreach (var jobId in savedJobIds) RecurringJob.RemoveIfExists(jobId);
        }
    }

    /// <summary>
    ///     Belirtilen tekrarlanan işin olup olmadığını belirlemek için kullanılır.
    /// </summary>
    /// <param name="recurringJobId">Job Unique identifier bilgisi.</param>
    /// <returns>Belirtielen tekrarlanan işin olup olmadığı bilgisi.</returns>
    public bool RecurringJobExists(string recurringJobId, string queueName = "default")
    {
        var connection = JobStorage.Current.GetConnection();
        var recurringJob = connection.GetRecurringJobs()
            .FirstOrDefault(job => job.Id.Equals(recurringJobId) && job.Queue.Equals(queueName));

        return recurringJob != null;
    }

    /// <summary>
    ///     Cron expression ı okunabilir bir formata dönüştürmek için kullanılır.
    /// </summary>
    /// <param name="cron">Cron expression.</param>
    /// <returns>Cron expression ın okunabilir hali.</returns>
    public string DescribeCron(string cron)
    {
        return CronDescriptor.DescribeCron(cron);
    }

    /// <summary>
    ///     Cron expressionı parse etmek için kullanılır.
    /// </summary>
    /// <param name="cron">Cron expression.</param>
    /// <returns>Cron expression ın CronConfiguration object olarak parse halini döndürür.</returns>
    public CronConfiguration ParseCron(string cron)
    {
        return CronDescriptor.ParseCron(cron);
    }
}