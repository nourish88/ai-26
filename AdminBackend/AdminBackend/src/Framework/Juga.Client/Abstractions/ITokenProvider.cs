using System.ComponentModel;

namespace Juga.Client.Abstractions;

public interface ITokenProvider
{
    Task<string> GetToken(TokenType type);
    Task<string> GetToken(string authenticationSchema, TokenType type);
}

public enum TokenType
{
    [Description("access_token")]
    AccessToken,
    [Description("id_token")]
    IdToken,
    [Description("refresh_token")]
    RefreshToken
}