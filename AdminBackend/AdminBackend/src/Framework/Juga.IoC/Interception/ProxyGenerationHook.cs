using System.Reflection;
using Castle.DynamicProxy;
using Juga.Abstractions.Ioc;

namespace Juga.IoC.Interception;

internal class ProxyGenerationHook : IProxyGenerationHook
{
    public void MethodsInspected()
    {
    }

    public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
    {
    }

    public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
    {
        return methodInfo.GetCustomAttributes().Any(m => m is InterceptionAttribute);
    }
}