using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AdminBackend.Infrastructure.Data
{
    public class JugaAIDbContextFactory : IDesignTimeDbContextFactory<JugaAIDbContext>
    {
        public JugaAIDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json")
                .Build();

            if (configuration == null)
            {
                throw new Exception("Configuration not found");
            }
            var connectionString = configuration.GetConnectionString("Database");
            if (connectionString == null)
            {
                throw new Exception($"Connection string not found in configuration at {Directory.GetCurrentDirectory()}");
            }
            var optionsBuilder = new DbContextOptionsBuilder<JugaAIDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return new JugaAIDbContext(optionsBuilder.Options);
        }
    }
}
