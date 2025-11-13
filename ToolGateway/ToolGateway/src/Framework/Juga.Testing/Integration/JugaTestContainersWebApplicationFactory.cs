using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Juga.Testing.Integration
{
    public abstract class JugaTestContainersWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        protected string TestSuitName;
        protected List<IContainer> containers = new List<IContainer>(30);
        public JugaTestContainersWebApplicationFactory()
        {
            TestSuitName = typeof(TProgram).FullName ?? typeof(TProgram).Name;
            this.ConfigureAwait(false);
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Use Test Containers Environment for the SUT
            builder.UseEnvironment("TC");

            // To throw the startup exceptions must be set to false
            builder.CaptureStartupErrors(false);

            //base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                // Test Fixture or Test Suit Specific Implementation
                ConfigureTestServices(services);

                services.AddAuthentication(options =>
                {
                    options.DefaultScheme = TestAuthenticationHandler.AuthenticationSchemeName;
                    options.DefaultChallengeScheme = TestAuthenticationHandler.AuthenticationSchemeName;
                    options.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthenticationSchemeName;
                }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(TestAuthenticationHandler.AuthenticationSchemeName,
                    schemeOptions => { });

            });
        }

        /// <summary>
        /// Test Fixture or Test Suit Specific Implementation
        /// </summary>
        /// <param name="services"></param>
        public abstract void ConfigureTestServices(IServiceCollection services);

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            var testServer = base.CreateServer(builder);

            // TODO: Things needs to be done before returning the TestServer

            return testServer;
        }

        public void AddContainer(IContainer container)
        {
            containers.Add(container);
        }

        public async Task InitializeAsync()
        {
            foreach(var container in containers)
            {
                await container.StartAsync();
            }
        }

        public new async Task DisposeAsync()
        {
            foreach (var container in containers)
            {
                await container.StopAsync();
            }
        }
    }
}
