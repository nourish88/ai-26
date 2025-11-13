using Castle.DynamicProxy;
using Juga.Abstractions.Data.TransactionManagement;
using Juga.Abstractions.Helpers;
using Juga.IoC.Interception;
using System.Transactions;

namespace Juga.Data.TransactionManagement;

public class TransactionalInterceptor : IMethodInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var tranAttribute = GetTransactionalAttribute(invocation);
        if (tranAttribute != null)
        {
            if (invocation.Method.IsAsync())
            {
                PerformAsync(invocation, tranAttribute);
            }
            else
            {
                PerformSync(invocation, tranAttribute);
            }
        }
        else
        {
            invocation.Proceed();

        }
        return;
    }

    private static TransactionalAttribute GetTransactionalAttribute(IInvocation invocation)
    {
        var methodInfo = invocation.MethodInvocationTarget;
        if (methodInfo == null)
        {
            methodInfo = invocation.Method;
        }

        return (TransactionalAttribute)methodInfo.GetCustomAttributes(typeof(TransactionalAttribute), false).FirstOrDefault();
    }

    private void PerformAsync(IInvocation invocation, TransactionalAttribute tranAttribute)
    {
        var scopeOption = tranAttribute.TransactionScopeOption;
        var scope = TransactionScopeFactory.Create(scopeOption, tranAttribute.AllowedIsolationLevel, TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            invocation.Proceed();
        }
        catch (System.Exception)
        {
            scope.Dispose();
            throw;
        }

        if (invocation.Method.ReturnType == typeof(Task))
        {
            invocation.ReturnValue = AsyncHelper.AwaitTaskWithPostActionAndFinally(
                (Task)invocation.ReturnValue,
                async () => scope.Complete(),
                exception => scope.Dispose()
            );
        }
        else
        {
            invocation.ReturnValue = AsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                invocation.Method.ReturnType.GenericTypeArguments[0],
                invocation.ReturnValue,
                async () => scope.Complete(),
                exception => scope.Dispose()
            );
        }
    }

    private void PerformSync(IInvocation invocation, TransactionalAttribute tranAttribute)
    {
        var scopeOption = tranAttribute.TransactionScopeOption;
        using var scope = TransactionScopeFactory.Create(scopeOption, tranAttribute.AllowedIsolationLevel);
        invocation.Proceed();
        scope.Complete();
    }
}