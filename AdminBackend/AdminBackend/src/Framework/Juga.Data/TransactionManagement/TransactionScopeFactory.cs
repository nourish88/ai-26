using System.Transactions;

namespace Juga.Data.TransactionManagement;

internal class TransactionScopeFactory
{
    internal static TransactionScope Create(TransactionScopeOption scopeOption,
        AllowedIsolationLevel allowedIsolationLevel = AllowedIsolationLevel.ReadCommitted
        , TransactionScopeAsyncFlowOption asyncFlowOption = TransactionScopeAsyncFlowOption.Enabled)
    {
        return new TransactionScope(scopeOption,
            new TransactionOptions { IsolationLevel = GetIsolationLevel(allowedIsolationLevel) }, asyncFlowOption);
    }


    private static IsolationLevel GetIsolationLevel(AllowedIsolationLevel allowedIsolationLevel)
    {
        IsolationLevel isolationLevel = 0;
        switch (allowedIsolationLevel)
        {
            case AllowedIsolationLevel.ReadCommitted:
                isolationLevel = IsolationLevel.ReadCommitted;
                break;
            case AllowedIsolationLevel.ReadUncommitted:
                isolationLevel = IsolationLevel.ReadUncommitted;
                break;
            case AllowedIsolationLevel.RepeatableRead:
                isolationLevel = IsolationLevel.RepeatableRead;
                break;
            default:
                isolationLevel = IsolationLevel.ReadCommitted;
                break;
        }

        return isolationLevel;
    }
}