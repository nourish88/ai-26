using Juga.Abstractions.TaskScheduling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Juga.TaskScheduling.Hangfire.Configuration;

public static class TaskSchedulerServiceCollectionExtensions
{
    /// <summary>
    ///     TaskSchedulingOptions konfigurasyonunun kaydı için kullanılır.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureTaskScheduler(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTaskScheduler();
        return services.Configure<TaskSchedulingOptions>(
            configuration.GetSection(TaskSchedulingOptions.TaskSchedulingkOptionsSection));
    }

    /// <summary>
    ///     ITaskSchedulingEngine servisi ve bileşenlerini eklemek için kullanılır.
    /// </summary>
    public static void AddTaskScheduler(this IServiceCollection services)
    {
        services.AddSingleton<ITaskSchedulingEngine, HangfireTaskSchedulingEngine>();
    }
}