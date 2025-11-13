using IdentityModel;
using Juga.Abstractions.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Serilog.Context;
using Juga.Logging.Serilog.Enrichers;


namespace Juga.Mvc.Middlewares;

public class UserContextProviderMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IUserContextProvider userContextProvider)
    {
        SetClientInfo(context, userContextProvider);
        using (LogContext.Push(new UserContextEnricher(userContextProvider)))
        {
            await next(context);
        }
    }

    private async Task SetClientInfo(HttpContext context, IUserContextProvider userContextProvider)
    {
        userContextProvider.ClientId = context?.User?.Claims?.FirstOrDefault(m => m.Type == JwtClaimTypes.PreferredUserName)?.Value;
        var accessToken = await context.GetTokenAsync("access_token");
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(accessToken);
        var token = jsonToken as JwtSecurityToken;
        userContextProvider.IdentityNumber = token.Claims.FirstOrDefault(claim => claim.Type == "identity_number")?.Value;
        userContextProvider.Roles = token?.Claims?.Where(claim => claim.Type == "user_roles")?.Select(x=>x.Value)?.ToList();

        userContextProvider.Projects = token?.Claims?.Where(claim => claim.Type == "user_projects")?.Select(x => x.Value)?.ToList();
        userContextProvider.Email = token?.Claims?.FirstOrDefault(claim => claim.Type == "email")?.Value;
        userContextProvider.ClientName = context?.User?.Claims?.FirstOrDefault(m => m.Type == JwtClaimTypes.Name)?.Value;
        userContextProvider.UserCode = context?.User?.Claims?.FirstOrDefault(claim => claim.Type == "user_code")?.Value;
        userContextProvider.CorporateUser = context?.User?.Claims?.FirstOrDefault(claim => claim.Type == "kurumsalkullanici")?.Value;
        userContextProvider.TroopCode = context?.User?.Claims?.FirstOrDefault(claim => claim.Type == "troop_code")?.Value;
        userContextProvider.CityCode = context?.User?.Claims?.FirstOrDefault(claim => claim.Type == "city_code")?.Value;
        userContextProvider.DistrictCode = context?.User?.Claims?.FirstOrDefault(claim => claim.Type == "district_code")?.Value;
        userContextProvider.ClientIp = context.Connection.RemoteIpAddress?.ToString();
            
    }
}