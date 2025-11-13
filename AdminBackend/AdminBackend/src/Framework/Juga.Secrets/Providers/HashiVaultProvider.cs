using Juga.Abstractions.Secrets;
using Microsoft.Extensions.Configuration;
using VaultSharp;

namespace Juga.Secrets.Providers;

public class HashiVaultProvider(IVaultClient vaultClient, IConfiguration configuration) : IVaultProvider
{
    public async Task<object> GetValue(string key)
    {
        var store = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            configuration["Juga:KeyVault:SecretName"], null, configuration["Juga:KeyVault:SecretsEngine"]);
        return store.Data.Data.FirstOrDefault(p => p.Key == key).Value;
    }
}