using System.Reflection;
using Juga.Abstractions.TaskScheduling;

namespace Juga.TaskScheduling.Hangfire;

public class JobInitializationHelper
{
    public static void InitializeJobs(Assembly[] assemblies)
    {
        foreach (var assembly in assemblies) InitializeJobs(assembly);
    }

    private static void InitializeJobs(Assembly assembly)
    {
        var types = GetJobInitializerTypes(assembly);
        foreach (var type in types)
        {
            var jobInitializer = Activator.CreateInstance(type) as IJobInitializer;
            jobInitializer.Initilize();
        }
    }


    private static IEnumerable<Type> GetJobInitializerTypes(Assembly assembly)
    {
        return assembly.GetTypes().Where(x =>
            x != typeof(IJobInitializer) && typeof(IJobInitializer).IsAssignableFrom(x) && !x.IsAbstract);
    }
}