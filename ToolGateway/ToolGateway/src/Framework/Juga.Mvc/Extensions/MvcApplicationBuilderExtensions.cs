using HealthChecks.UI.Client;
using Juga.Abstractions.Logging;
using Juga.Logging.Serilog.Extensions;
using Juga.Mvc.ExceptionHandling;
using Juga.Mvc.Helpers;
using Juga.Mvc.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;



namespace Juga.Mvc.Extensions;

/// <summary>
/// Mvc uygulamaları için servislerin ayağa kaldırılması ve http pipeline konfigürasyonu işlemlerini framework düzeyinde toparlamak için kullanılır.
/// </summary>
public static class MvcApplicationBuilderExtensions
{
    /// <summary>
    /// Framework düzeyinde Http Pipeline konfigürasyonunu yapar.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    /// <param name="configuration"></param>
    /// <param name="options"></param>
    /// <param name="defaultController"></param>
    /// <param name="defaultAction"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseJugaMvc(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration, MvcOptions options, string defaultController="Home", string defaultAction="Index",bool? healthCheck=false, bool useEndPoints=true)
    {

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseMvcExceptionHandler(options => options.AddResponseDetails = new DefaultMvcExceptionOptions().AddResponseDetails);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        var forwardingOptions = new ForwardedHeadersOptions()
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        };
        forwardingOptions.KnownNetworks.Clear(); //its loopback by default
        forwardingOptions.KnownProxies.Clear();
        app.UseForwardedHeaders(forwardingOptions);
        app.UseCertificateForwarding();

        //app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.SetLogging(configuration);
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<UserContextProviderMiddleware>();
        if (useEndPoints)
        {
            app.UseEndpoints(endpoints =>
            {


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=" + defaultController + "}/{action=" + defaultAction + "}/{id?}");
                if (options.HubList != null)
                    foreach (var item in options.HubList)
                    {
                        var method = typeof(HubEndpointRouteBuilderExtensions).GetMethod("MapHub", new[] { typeof(IEndpointRouteBuilder), typeof(string) });
                        var generic = method.MakeGenericMethod(item);
                        generic.Invoke(null, new object[] { endpoints, $"/{item.Name}" });
                    }
                if (healthCheck == true)
                {
                    endpoints.MapHealthChecks("/health", new HealthCheckOptions
                    {
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                }
                    
                //endpoints.MapHub<THub>($"/{typeof(THub).Name}");
            });
        }

        return app;
    }

    private static void SetLogging(this IApplicationBuilder app, IConfiguration configuration)
    {
        var loggingOptions = new LoggingOptions();
        configuration.Bind(LoggingOptions.OptionsSection, loggingOptions);
        if (loggingOptions.EnableRequestResponseLogging)
        {
            if (loggingOptions.LoggerType == LoggerType.Serilog)
            {
                app.UseRequestResponseLogging();
            }
        }
    }
}