using Juga.Client.Abstractions;
using Juga.Client.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
namespace Juga.Client.Providers;

public class HttpContextTokenProvider(IHttpContextAccessor httpContextAccessor) : ITokenProvider
{
    public async Task<string> GetToken(TokenType type)
    {
        return await( httpContextAccessor.HttpContext == null ? Task.FromResult("") : httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, type.ToDescription()));
    }
}