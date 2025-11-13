using AdminBackend.Infrastructure.Data;
using Juga.Api.Enums;
using Juga.Api.Helpers;
using Juga.Api.Models;
using Juga.Data.Extensions;
using Juga.DataAudit.Common;
using System.Reflection;

namespace AdminBackend.Api
{
    public static class Module
    {
        private static readonly Lazy<ApiOptions> Options = new(() => new ApiOptions
        {
            AuditLogStoreType = AuditLogStoreType.PostgreSql,
            Mediator = true,
            IsMinimal = true
        });

        private static WebApiStartUpConfig? _config;

        private static ApiOptions options => Options.Value;

        private static WebApiStartUpConfig GetConfig(WebApplicationBuilder builder)
        {
            if (_config == null || _config.Builder != builder)
            {
                _config = new WebApiStartUpConfig(
                    Builder: builder,
                    ConnectionStringName: "Database",
                    BaseDbProvider: EfCoreDbProviders.PostgreSql,
                    AuditLogStoreType: AuditLogStoreType.PostgreSql,
                    IsMinimal: true,
                    MigrationAssemblyName: "AdminBackend.Infrastructure");
            }
            return _config;
        }
        public static IServiceCollection AddModule(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var config = GetConfig(builder);

            //SerilogProgramRunnerExtensions.RunHost(builder);

            options.RegistrationAssemblies =
            [
                Assembly.GetExecutingAssembly()
            ];

            //Juga Service Registrations 
            ModulithProgramHelper.RegisterServices<JugaAIDbContext>(config, options);

            //  services.AddScoped<IDataSeeder, CatalogDataSeeder>();
            return services;
        }
        public static IApplicationBuilder UseModule(this WebApplication app)
        {
            if (_config == null)
            {
                throw new InvalidOperationException("AddModule must be called before UseModule");
            }
            //app.UseMigration<JugaAIDbContext>();
            return app;
        }
    }
}
