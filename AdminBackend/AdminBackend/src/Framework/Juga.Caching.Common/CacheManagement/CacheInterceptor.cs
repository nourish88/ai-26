using Castle.DynamicProxy;
using Juga.Abstractions.Caching;
using Juga.Abstractions.Caching.CacheManagement;
using Juga.Abstractions.Helpers;
using Juga.IoC.Interception;

namespace Juga.Caching.Common.CacheManagement;

public class CacheInterceptor(ICacheManager<object> cacheManager) : IMethodInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var cacheAttribute = GetCacheAttribute(invocation);
        if (cacheAttribute != null)
        {
            if (invocation.Method.IsAsync())
                PerformAsync(invocation, cacheAttribute);
            else
                PerformSync(invocation, cacheAttribute);
        }
        else
        {
            invocation.Proceed();
        }
    }

    private static CacheAttribute GetCacheAttribute(IInvocation invocation)
    {
        var methodInfo = invocation.MethodInvocationTarget;
        if (methodInfo == null) methodInfo = invocation.Method;

        return (CacheAttribute)methodInfo.GetCustomAttributes(typeof(CacheAttribute), false).FirstOrDefault();
    }

    private void PerformAsync(IInvocation invocation, CacheAttribute cacheAttribute)
    {
        var cachedItem = ReadFromCache(invocation, cacheAttribute, out var key);
        if (cachedItem != null)
        {
            var returnTask = AsyncHelper.ConvertTaskType(Task.FromResult(cachedItem),
                invocation.Method.ReturnType.GenericTypeArguments[0]);
            invocation.ReturnValue = returnTask;
        }
        else
        {
            invocation.Proceed();
            AsyncHelper.CallAwaitTaskWithPostActionWithResultAndFinallyAndGetResult(
                invocation.Method.ReturnType.GenericTypeArguments[0],
                invocation.ReturnValue,
                res => { WriteToCahce(key, res, cacheAttribute); },
                ex => { }
            );
        }
    }

    private void PerformSync(IInvocation invocation, CacheAttribute cacheAttribute)
    {
        var cachedItem = ReadFromCache(invocation, cacheAttribute, out var key);
        if (cachedItem != null)
        {
            invocation.ReturnValue = cachedItem;
        }
        else
        {
            invocation.Proceed();
            var returnValue = invocation.ReturnValue;
            WriteToCahce(key, returnValue, cacheAttribute);
        }
    }

    private object ReadFromCache(IInvocation invocation, CacheAttribute cacheAttribute, out string key)
    {
        key = GetKey(invocation, cacheAttribute);
        return cacheManager.Get(key, cacheAttribute.Region);
    }

    private void WriteToCahce(string key, object value, CacheAttribute cacheAttribute)
    {
        if (value == null) return;
        if (!string.IsNullOrWhiteSpace(cacheAttribute.Policy))
            cacheManager.Add(key, value, cacheAttribute.Policy, cacheAttribute.Region);
        else
            cacheManager.Add(key, value, cacheAttribute.Expire, cacheAttribute.ExpirationMode, cacheAttribute.Region);
    }

    private string GetKey(IInvocation invocation, CacheAttribute cacheAttribute)
    {
        if (cacheAttribute.CacheKeySuffixSelector != null)
            return string.Concat(cacheAttribute.Key,
                cacheAttribute.CacheKeySuffixSelector.GetSuffix(invocation.Arguments));
        return cacheAttribute.Key;
    }
}