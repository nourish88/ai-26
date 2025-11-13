using AdminBackend.Application.Services.Oidc;
using IdentityModel.Client;

namespace AdminBackend.Infrastructure.Services.Oidc
{
    //NOTE : Keycloak service account must be enabled
    public class KeycloakOidcClient : IOidcService
    {
        private readonly HttpClient _httpClient;
        
        public KeycloakOidcClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetTokenAsync(
            string authority,
            string clientId,
            string clientSecret,
            string? scope = null)
        {
            
            var discovery = await _httpClient.GetDiscoveryDocumentAsync(
                new DiscoveryDocumentRequest
                {
                    Address = $"{authority}/.well-known/openid-configuration",
                    Policy = new DiscoveryPolicy { RequireHttps = authority.StartsWith("https://") }
                });

            if (discovery.IsError)
                throw new Exception($"Discovery error: {discovery.Error}");

            var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discovery.TokenEndpoint,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = scope
                });

            if (tokenResponse.IsError)
                throw new Exception($"Token error: {tokenResponse.Error}");

            return tokenResponse?.AccessToken;
        }
    }
}
