using System.Reflection;

namespace Juga.Abstractions.Helpers;

/// <summary>
///     Yalnızca framework tarafından kullanılıyor, metodların async olup olmadığını async ise devamında başka bir async
///     var mı kontrol için
/// </summary>
public static class AsyncHelper
{
    public static bool IsAsync(this MethodInfo method)
    {
        return method.ReturnType == typeof(Task) ||
               (method.ReturnType.GetTypeInfo().IsGenericType &&
                method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
    }

    public static async Task AwaitTaskWithPostActionAndFinally(Task actualReturnValue, Func<Task> postAction,
        Action<Exception> finalAction)
    {
        Exception? exception = null;

        try
        {
            await actualReturnValue;
            await postAction();
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            finalAction(exception?? new Exception());
        }
    }

    public static async Task<T> AwaitTaskWithPostActionAndFinallyAndGetResult<T>(Task<T> actualReturnValue,
        Func<Task> postAction, Action<Exception> finalAction)
    {
        Exception? exception = null;

        try
        {
            var result = await actualReturnValue;
            await postAction();
            return result;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            finalAction(exception?? new Exception());
        }
    }

    public static async Task<T> AwaitTaskWithPostActionWithResultAndFinallyAndGetResult<T>(Task<T> actualReturnValue,
        Action<T> postAction, Action<Exception> finalAction)
    {
        Exception? exception = null;

        try
        {
            var result = await actualReturnValue;
            postAction(result);
            return result;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            finalAction(exception??new Exception());
        }
    }

    public static object CallAwaitTaskWithPostActionAndFinallyAndGetResult(Type taskReturnType,
        object actualReturnValue, Func<Task> action, Action<Exception> finalAction)
    {
        return typeof(AsyncHelper)
            .GetMethod("AwaitTaskWithPostActionAndFinallyAndGetResult", BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(taskReturnType)!
            .Invoke(null, [actualReturnValue, action, finalAction])!;
    }

    public static object CallAwaitTaskWithPostActionWithResultAndFinallyAndGetResult(Type taskReturnType,
        object actualReturnValue, Action<object> postAction, Action<Exception> finalAction)
    {
        return typeof(AsyncHelper)
            .GetMethod("AwaitTaskWithPostActionWithResultAndFinallyAndGetResult",
                BindingFlags.Public | BindingFlags.Static)!
            .MakeGenericMethod(taskReturnType)
            .Invoke(null, [actualReturnValue, postAction, finalAction])!;
    }

    public static object ConvertTaskType(Task<object> sourceTask, Type requirdTaskType)
    {
        var type = typeof(TaskCompletionSource<>).MakeGenericType(requirdTaskType);
        var setResultMethod = type.GetMethod("SetResult");
        var setExceptionMethod = type.GetMethod("SetException", [typeof(Exception)]);
        var taskProperty = type.GetProperty("Task");

        var taskCompletionSource = Activator.CreateInstance(type);
        Task.Factory.StartNew(async () =>
            await SetResultAndException(setResultMethod!, setExceptionMethod!, sourceTask, taskCompletionSource!));

        return GetTask(taskProperty!, taskCompletionSource!);
    }

    private static async Task SetResultAndException(MethodInfo setResultMethod, MethodInfo setExceptionMethod,
        Task<object> sourceTask, object targetTask)
    {
        try
        {
            SetResult(setResultMethod, targetTask, await sourceTask);
        }
        catch (Exception ex)
        {
            SetException(setExceptionMethod, targetTask, ex);
        }
    }

    private static void SetResult(MethodInfo setResultMethod, object taskCompletionSource, object result)
    {
        setResultMethod.Invoke(taskCompletionSource, [result]);
    }

    private static void SetException(MethodInfo setExceptionMethod, object taskCompletionSource, Exception ex)
    {
        setExceptionMethod.Invoke(taskCompletionSource, [ex]);
    }

    private static object GetTask(PropertyInfo getTaskProperty, object taskCompletionSource)
    {
        return getTaskProperty.GetValue(taskCompletionSource)!;
    }
}