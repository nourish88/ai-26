using Juga.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Juga.Testing.Integration
{
    public abstract class JugaInMemoryWebApplicationFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram> where TProgram : class where TDbContext : UnitOfWork
    {
        protected string TestSuitName;
        public JugaInMemoryWebApplicationFactory()
        {
            TestSuitName = typeof(TProgram).FullName ?? typeof(TProgram).Name;
            this.ConfigureAwait(false);
        }

        public JugaInMemoryWebApplicationFactory(string testSuitName)
        {
            TestSuitName = testSuitName;
            this.ConfigureAwait(false);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Use Integration Testing Environment for the SUT
            builder.UseEnvironment("IT");

            builder.CaptureStartupErrors(false);

            //base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                // Replace Real DB Context with InMemory DbContext
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<TDbContext>));

                if (dbContextDescriptor != null)
                {
                    // Remove real DbContext
                    services.Remove(dbContextDescriptor);
                    // Add Test DbContext
                    services.AddDbContext<TDbContext>((serviceProvider, options) =>
                    {
                        options.UseInMemoryDatabase(TestSuitName, inMemoryDatabaseOptions =>
                        {
                            inMemoryDatabaseOptions.EnableNullChecks();
                        });
                    });
                }
                else
                {
                    throw new InvalidOperationException($"Application didn't register the DbContextOptions for {nameof(TDbContext)}");
                }

                // Configure additonal Test Services
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

            using var dbContext = CreateDbContext(testServer.Host.Services);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            return testServer;
        }

        public static TDbContext CreateDbContext(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
            return dbContext;
        }

    }
}
