namespace Juga.Abstractions.Secrets;

public interface IVaultProvider
{
    Task<object> GetValue(string key);
}