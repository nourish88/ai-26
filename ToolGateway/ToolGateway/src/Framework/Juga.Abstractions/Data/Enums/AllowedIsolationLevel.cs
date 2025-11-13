namespace Juga.Abstractions.Data.Enums;

/// <summary>
/// Transaction isolation level tipleri
/// Bkz. https://docs.microsoft.com/en-us/sql/t-sql/statements/set-transaction-isolation-level-transact-sql?view=sql-server-ver15
/// </summary>
public enum AllowedIsolationLevel
{
    RepeatableRead,
    ReadUncommitted,
    ReadCommitted
}

//TODO: https://docs.microsoft.com/en-us/dotnet/api/system.data.isolationlevel?view=netcore-3.1 bu sınıf kullanılabilir mi?