using Juga.Abstractions.Data.Enums;
using Juga.Abstractions.Ioc;
using System.Transactions;

namespace Juga.Abstractions.Data.TransactionManagement
{
    /// <summary>
    /// Transaction kullanılması gereken metodlara attribute olarak eklenecek
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class TransactionalAttribute(TransactionScopeOption scopeOption = TransactionScopeOption.Required,
            AllowedIsolationLevel isolationLevel = AllowedIsolationLevel.ReadCommitted)
        : InterceptionAttribute
    {
        public AllowedIsolationLevel AllowedIsolationLevel { get; set; } = isolationLevel;

        public TransactionScopeOption TransactionScopeOption { get; set; } = scopeOption;
    }
}