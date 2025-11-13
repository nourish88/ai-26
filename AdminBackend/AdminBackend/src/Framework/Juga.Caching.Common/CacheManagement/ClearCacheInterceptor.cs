using Castle.DynamicProxy;
using Juga.Abstractions.Caching;
using Juga.Abstractions.Caching.CacheManagement;
using Juga.Abstractions.Helpers;
using Juga.IoC.Interception;

namespace Juga.Caching.Common.CacheManagement;

public class ClearCacheInterceptor(ICacheManager<object> cacheManager) : IMethodInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var clearCacheAttribute = GetClearCacheAttribute(invocation);
        if (clearCacheAttribute != null)
        {
            if (invocation.Method.IsAsync())
            {
                PerformAsync(invocation, clearCacheAttribute);
            }
            else
            {
                PerformSync(invocation, clearCacheAttribute);
            }
        }
        else
        {
            invocation.Proceed();

        }
    }

    private static ClearCacheAttribute GetClearCacheAttribute(IInvocation invocation)
    {
        var methodInfo = invocation.MethodInvocationTarget;
        if (methodInfo == null)
        {
            methodInfo = invocation.Method;
        }

        return (ClearCacheAttribute)methodInfo.GetCustomAttributes(typeof(ClearCacheAttribute), false).FirstOrDefault();
    }

    private void PerformAsync(IInvocation invocation, ClearCacheAttribute clearCacheAttribute)
    {
            
        invocation.Proceed();
        AsyncHelper.CallAwaitTaskWithPostActionWithResultAndFinallyAndGetResult(
            invocation.Method.ReturnType.GenericTypeArguments[0],
            invocation.ReturnValue,
            (res) => { ClearCache(invocation, clearCacheAttribute); },
            (ex) => { }
        );
    }

    private void PerformSync(IInvocation invocation, ClearCacheAttribute clearCacheAttribute)
    {
        invocation.Proceed();
        ClearCache(invocation, clearCacheAttribute);
    }

    private void ClearCache(IInvocation invocation, ClearCacheAttribute clearCacheAttribute)
    {
        var key = GetKey(invocation, clearCacheAttribute);
        cacheManager.Remove(key, clearCacheAttribute.Region);
    }

    private string GetKey(IInvocation invocation, ClearCacheAttribute clearCacheAttribute)
    {
        return clearCacheAttribute.CacheKeySuffixSelector != null ? string.Concat(clearCacheAttribute.Key, clearCacheAttribute.CacheKeySuffixSelector.GetSuffix(invocation.Arguments)) : clearCacheAttribute.Key;
    }
}