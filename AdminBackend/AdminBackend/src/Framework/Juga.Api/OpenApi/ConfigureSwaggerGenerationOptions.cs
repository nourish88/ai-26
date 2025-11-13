using Asp.Versioning.ApiExplorer;
using IdentityModel.Client;
using Juga.Auth.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Juga.Api.OpenApi;

/// <summary>
///     Swagger dokümanları için authorization flow tanımını yapar.
/// </summary>
public class ConfigureSwaggerGenerationOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApiVersionDescriptionProvider _descriptionProvider;
    

    /// <summary>
    ///     Bağımlılıkları başlatır.
    /// </summary>
    /// <param name="configuration">Konfigürasyondan gelecek identity provider bilgileri için kullanlır</param>
    /// <param name="httpClientFactory">Identity provider keşfi için kullanılır</param>
    public ConfigureSwaggerGenerationOptions(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory, IApiVersionDescriptionProvider descriptionProvider)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _descriptionProvider = descriptionProvider;
    }

    /// <summary>
    ///     Swagger içi authorization grant flow konfigürasyonunu yapar
    /// </summary>
    /// <param name="options"></param>
    public void Configure(SwaggerGenOptions options)
    {
        var discoveryDocument = GetDiscoveryDocument();

        options.OperationFilter<AuthorizeOperationFilter>();
        options.DescribeAllParametersInCamelCase();
        options.CustomSchemaIds(x => x.GenericsSupportedId());
        foreach (var description in _descriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateOpenApiInfo(description));
        }
        options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,

            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    //AuthorizationUrl = new Uri(discoveryDocument.AuthorizeEndpoint),
                    //TokenUrl = new Uri(discoveryDocument.TokenEndpoint),
                    AuthorizationUrl =
                        new Uri($"{_configuration["Juga:Security:Jwt:Authority"]}/protocol/openid-connect/auth"),
                    TokenUrl = new Uri($"{_configuration["Juga:Security:Jwt:Authority"]}/protocol/openid-connect/token")
                    //Scopes = new Dictionary<string, string>
                    //{
                    //    { _settings.Security.Jwt.Audience , "Balea Server HTTP Api" }
                    //},
                }
            },
            Description = "Balea Server OpenId Security Scheme"
        });
    }

    /// <summary>
    ///     Identity provider keşfini yapar. JWKS adresi vs.
    /// </summary>
    /// <returns></returns>
    private DiscoveryDocumentResponse GetDiscoveryDocument()
    {
        return _httpClientFactory
            .CreateClient()
            .GetDiscoveryDocumentAsync(_configuration["Juga:Security:Jwt:Authority"])
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    ///     Swagger dokümantasyonu versiyon bilgisi gibi tanımları getirir
    /// </summary>
    /// <returns></returns>
    private OpenApiInfo CreateOpenApiInfo(ApiVersionDescription apiVersionDescription)
    {
        //TODO: Bu kısımlar konfigürasyon dosyasından gelmeli
        return new OpenApiInfo
        {
            Title = _configuration["Juga:OpenApi:Name"] ?? "My Awesome API",
            Version = apiVersionDescription.ApiVersion.ToString(),
            Description = _configuration["Juga:OpenApi:Name"] ?? "My Awesome API",
            Contact = new OpenApiContact { Name = "API" },
            License = new OpenApiLicense()
        };
    }
}