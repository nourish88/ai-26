using System.Net;
using System.Text;
using FluentValidation.AspNetCore;
using IdentityModel.Client;
using Juga.Abstractions.Client;
using Juga.Abstractions.Mvc;
using Juga.Client.Abstractions;
using Juga.Client.Models.Service;
using Juga.Client.Providers;
using Juga.Client.SignalR.Abstractions;
using Juga.Client.SignalR.Hub;
using Juga.Mvc.Helpers;
using Juga.Mvc.Providers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using StackExchange.Redis;

namespace Juga.Mvc.Extensions;

/// <summary>
///     Mvc uygulamaları için servislerin ayağa kaldırılması ve http pipeline konfigürasyonu işlemlerini framework
///     düzeyinde toparlamak için kullanılır.
/// </summary>
public static class MvcServiceCollectionExtensions
{
    /// <summary>
    ///     Framework düzeyinde servislerin ayağa kaldırılması işini yapar.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddJugaMvc(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment env, MvcOptions options = null)
    {
        services.AddControllersWithViews().AddFluentValidation(fv =>
        {
            if (options != null)
                foreach (var assembly in options.RegistrationAssemblies)
                    fv.RegisterValidatorsFromAssembly(assembly);
        });
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IUserContextProvider, UserContextProvider>();


        //services.ConfigureApplicationCookie(options =>
        //{
        //    options.Cookie.Name = "MyCookieNameIPC";
        //});
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                // TODO 2
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Unspecified;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                // TODO 2

                options.Events = new CookieAuthenticationEvents
                {
                    // After the auth cookie has been validated, this event is called.
                    // In it we see if the access token is close to expiring.  If it is
                    // then we use the refresh token to get a new access token and save them.
                    // If the refresh token does not work for some reason then we redirect to 
                    // the login screen.
                    OnValidatePrincipal = async cookieCtx =>
                    {
                        var now = DateTimeOffset.UtcNow;
                        var expiresAt = cookieCtx.Properties.GetTokenValue("expires_at");
                        var accessTokenExpiration = DateTimeOffset.Parse(expiresAt);
                        var timeRemaining = accessTokenExpiration.Subtract(now);
                        // TODO: Get this from configuration with a fall back value.
                        var refreshThresholdSeconds = 20;
                        var refreshThreshold = TimeSpan.FromSeconds(refreshThresholdSeconds);

                        if (timeRemaining < refreshThreshold)
                        {
                            var refreshToken = cookieCtx.Properties.GetTokenValue("refresh_token");
                            // TODO: Get this HttpClient from a factory
                            var response = await new HttpClient().RequestRefreshTokenAsync(new RefreshTokenRequest
                            {
                                Address = configuration["Juga:Security:Jwt:Authority"],
                                ClientId = configuration["Juga:Security:Jwt:ClientId"],
                                ClientSecret = configuration["Juga:Security:Jwt:ClientSecret"],
                                RefreshToken = refreshToken
                            });

                            if (!response.IsError)
                            {
                                var expiresInSeconds = response.ExpiresIn;
                                var updatedExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds);
                                cookieCtx.Properties.UpdateTokenValue("expires_at", updatedExpiresAt.ToString());
                                cookieCtx.Properties.UpdateTokenValue("access_token", response.AccessToken);
                                cookieCtx.Properties.UpdateTokenValue("refresh_token", response.RefreshToken);

                                // Indicate to the cookie middleware that the cookie should be remade (since we have updated it)
                                cookieCtx.ShouldRenew = true;
                            }
                            else
                            {
                                cookieCtx.RejectPrincipal();
                                await cookieCtx.HttpContext.SignOutAsync();
                            }
                        }
                    }
                };
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
            {
                o.Authority = configuration["Juga:Security:Jwt:Authority"];
                //o.CallbackPath = new PathString("/abc");
                o.ClientId = configuration["Juga:Security:Jwt:ClientId"];
                o.ClientSecret = configuration["Juga:Security:Jwt:ClientSecret"];
                o.ResponseType = "code";
                o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.UseTokenLifetime = true;
                o.SaveTokens = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.GetValue<string>("Juga:Security:Jwt:ClientSecret")))
                };

                //end - not sure what RedirectUri is, but PostLogoutRedirectUri doesn't matter
                o.RequireHttpsMetadata = false; //fix to runtime error           //TODO 4          
                o.Events = new OpenIdConnectEvents
                {
                    OnTokenResponseReceived = context => { return Task.CompletedTask; },
                    OnTokenValidated = context => { return Task.CompletedTask; },
                    OnRedirectToIdentityProvider = context => { return Task.CompletedTask; },
                    OnRedirectToIdentityProviderForSignOut = context =>
                    {
                        //var logoutUri =
                        //    $"https://cimpress.auth0.com/v2/logout?client_id={settings.ClientKeyIdentifier}";
                        //var postLogoutUri = context.Properties.RedirectUri;
                        //if (!string.IsNullOrEmpty(postLogoutUri))
                        //{
                        //    if (postLogoutUri.StartsWith("/"))
                        //    {
                        //        var request = context.Request;
                        //        postLogoutUri =
                        //            $"{request.Scheme}://{request.Host}{request.PathBase}{postLogoutUri}";
                        //    }

                        //    logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                        //}
                        //context.Response.Redirect(logoutUri);
                        //context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
        services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(60); });

        services.AddAutoMapper(options.RegistrationAssemblies);
        if (configuration.GetValue<bool>("Juga:SignalR:EnableScaling"))
        {
            services.AddSignalR().AddStackExchangeRedis(o =>
            {
                o.ConnectionFactory = async writer =>
                {
                    var config = new ConfigurationOptions
                    {
                        AbortOnConnectFail = false
                    };
                    config.EndPoints.Add(
                        IPAddress.Parse(configuration.GetValue<string>("Juga:SignalR:StateStore:Address")),
                        configuration.GetValue<int>("Juga:SignalR:StateStore:Port"));
                    config.SetDefaultPorts();
                    var connection = await ConnectionMultiplexer.ConnectAsync(config, writer);
                    connection.ConnectionFailed += (_, e) => { Console.WriteLine("Connection to Redis failed."); };

                    if (!connection.IsConnected) Console.WriteLine("Did not connect to Redis.");

                    return connection;
                };
            });

            services.AddSingleton<IUserIdProvider, UserIdProvider>();
        }
        else
        {
            services.AddSignalR();

            services.AddSingleton<IUserIdProvider, UserIdProvider>();
        }


        services.AddTransient<IHubClient, HubClient>();

        var backends = configuration.GetSection("Juga:Service:Backends").Get<BackendModel[]>();
        if (backends != null)
        {
            foreach (var section in backends)
            {
                var httpClientConfigration = services
                    .AddHttpClient(section.Name, c => { c.BaseAddress = new Uri(section.Address); }).
                    //AddPolicyHandler(p => HttpPolicyExtensions
                    //.HandleTransientHttpError()
                    //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                    //.WaitAndRetryAsync(6, retryAttempt =>
                    //    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt
                    //)))).
                    AddPolicyHandler(p => Policy
                        .Handle<Exception>(res => p.Method == HttpMethod.Get || p.Method == HttpMethod.Put)
                        .OrTransientHttpStatusCode()
                        .WaitAndRetryAsync(0, retryAttempt =>
                            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt
                            )))).AddPolicyHandler(p => HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .CircuitBreakerAsync(1, TimeSpan.FromSeconds(30)));
                if (env.IsDevelopment())
                    httpClientConfigration.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        ClientCertificateOptions = ClientCertificateOption.Manual,
                        ServerCertificateCustomValidationCallback =
                            (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
                    });
            }

            services.AddTransient<ITokenProvider, HttpContextTokenProvider>();
            services.AddTransient<IHttpClientProvider, HttpClientProvider>();
        }


        //var connection = new HubConnectionBuilder()
        //.WithUrl("https://localhost:44301/SampleHub/", options =>
        //{
        //    options.AccessTokenProvider = () =>
        //    {
        //        var provider = services.BuildServiceProvider();
        //        return provider.GetService<ITokenProvider>().GetToken(TokenType.AccessToken);
        //    };
        //})
        //.WithAutomaticReconnect()
        //.Build();
        //services.AddSingleton(connection);
        services.AddScoped<IUserMessageService, UserMessageService>();
        return services;
    }
}