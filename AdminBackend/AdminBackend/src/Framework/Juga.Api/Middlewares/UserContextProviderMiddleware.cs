using IdentityModel;
using Juga.Logging.Serilog.Enrichers;
using Serilog.Context;
using System.IdentityModel.Tokens.Jwt;

namespace Juga.Api.Middlewares;

public class UserContextProviderMiddleware(RequestDelegate next)
{
    // Dependency Injection

    public async Task InvokeAsync(HttpContext context, IUserContextProvider userContextProvider)
    {
        SetClientInfo(context, userContextProvider);
        using (LogContext.Push(new UserContextEnricher(userContextProvider)))
        {
            await next(context);
        }
    }

    private void SetClientInfo(HttpContext context, IUserContextProvider userContextProvider)
    {
        string? authHeader = context.Request.Headers["Authorization"];
        if (authHeader != null)
        {
            authHeader = authHeader.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(authHeader) as JwtSecurityToken;
            if (token != null)
            {
                userContextProvider.ClientId = token.Claims
                    .FirstOrDefault(claim => claim.Type == JwtClaimTypes.PreferredUserName)?.Value;

                userContextProvider.ClientName =
                    token.Claims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.Name)?.Value;
                userContextProvider.UserCode = token.Claims.FirstOrDefault(claim => claim.Type == "user_code")?.Value;
                userContextProvider.Email = token.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
                userContextProvider.CorporateUser =
                    token.Claims.FirstOrDefault(claim => claim.Type == "kurumsalkullanici")?.Value;
                userContextProvider.TroopCode = token.Claims.FirstOrDefault(claim => claim.Type == "troop_code")?.Value;
                userContextProvider.CityCode = token.Claims.FirstOrDefault(claim => claim.Type == "city_code")?.Value;
                userContextProvider.DistrictCode =
                    token.Claims.FirstOrDefault(claim => claim.Type == "district_code")?.Value;
                var forwardedHeader = GetIPAddress(context);
                userContextProvider.ClientIp = !string.IsNullOrEmpty(forwardedHeader)
                    ? forwardedHeader.Split(',').First().Trim()
                    : context.Connection.RemoteIpAddress?.ToString();
                userContextProvider.IdentityNumber =
                    token.Claims.FirstOrDefault(claim => claim.Type == "identity_number")?.Value;
                userContextProvider.Roles = token?.Claims?.Where(claim => claim.Type == "user_roles")
                    ?.Select(x => x.Value)?.ToList();

                userContextProvider.Projects = token?.Claims?.Where(claim => claim.Type == "user_projects")
                    ?.Select(x => x.Value)?.ToList();
            }
        }

       

    }
    private string GetIPAddress(HttpContext? context)
    {
        if (context == null) return "127.0.0.1";
        string? ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(ip))
        {
            return ip;
        }
        string? realip = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realip))
        {
            return realip;
        }
        string? ipAddress = context.GetServerVariable("HTTP_X_FORWARDED_FOR");

        if (!string.IsNullOrEmpty(ipAddress))
        {
            string[] addresses = ipAddress.Split(',');
            if (addresses.Length != 0)
            {
                return addresses[0];
            }
        }
        string? remoteAddr = context.GetServerVariable("REMOTE_ADDR");
        if (!string.IsNullOrEmpty(remoteAddr))
        {
            return remoteAddr;
        }

        string remoteIpAddres = context.Connection.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(remoteIpAddres))
        {
            return remoteIpAddres;
        }
        return "127.0.0.1";
    }
}