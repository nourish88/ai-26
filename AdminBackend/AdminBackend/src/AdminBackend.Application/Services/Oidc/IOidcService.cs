namespace AdminBackend.Application.Services.Oidc
{
    public interface IOidcService
    {
        public Task<string?> GetTokenAsync(string authority, string clientId, string clientSecret, string? scope = null);
    }
}
