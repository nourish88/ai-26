using AdminBackend.Application.Services.DataManager;
using AdminBackend.Application.Services.Oidc;
using Juga.Client.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AdminBackend.Infrastructure.Services.DataManager;

public class DataManagerService : IDataManagerService
{
    private readonly ILogger<DataManagerService> logger;
    private HttpClient httpClient;
    private readonly ITokenProvider tokenProvider;
    private readonly IOidcService oidcService;
    private readonly IConfiguration configuration;
    private bool isAuthorizationHeaderAdded = false;

    public DataManagerService(ILogger<DataManagerService> logger,   
        HttpClient httpClient, 
        ITokenProvider tokenProvider, 
        IOidcService oidcService, 
        IConfiguration configuration)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        this.tokenProvider = tokenProvider;
        this.oidcService = oidcService;
        this.configuration = configuration;
    }

    public async Task SendJobRequest(JobRequest request, CancellationToken cancellationToken)
    {
        await AddAuthorizationHeader();
        var response = await httpClient.PostAsJsonAsync("job", request, cancellationToken);
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex,"Create job failed.");
        }
    }

    private async Task AddAuthorizationHeader()
    {
        if (!isAuthorizationHeaderAdded)
        {
            string? token = string.Empty;
            try
            {
                token = await tokenProvider.GetToken(JwtBearerDefaults.AuthenticationScheme, TokenType.AccessToken);
            }
            catch (Exception ex)
            {

            }

            if (string.IsNullOrEmpty(token))
            {
                var authority = configuration["Juga:Security:Jwt:Authority"];
                var clientId = configuration["Juga:Security:Jwt:ClientId"];
                var clientSecret = configuration["Juga:Security:Jwt:ClientSecret"];
                if (string.IsNullOrEmpty(authority) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    throw new Exception("Oidc service parameters are invalid.");
                }
                //TODO : Cache token
                token = await oidcService.GetTokenAsync(authority, clientId, clientSecret);
                
            }

            if (!string.IsNullOrEmpty(token))
            {
                if (token != null && !token.Contains("Bearer"))
                {
                    token = $"Bearer {token}";
                }
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", token);
            }
            isAuthorizationHeaderAdded = true;
        }
    }
}