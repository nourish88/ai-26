
using Microsoft.AspNetCore.Builder;

using Juga.Data.Seed;

namespace Juga.Data.Extensions;
/// <summary>
/// It helps app apply migrations whenever application starts up. So when deployed the database will be updated. So that you wont have to use dotnet ef database update in production environment.
/// </summary>
public static class MigrationExtensions
{
    public static IApplicationBuilder UseMigration<TContext>(this IApplicationBuilder app) where TContext : DbContext
    {
        MigrateDatabaseAsync<TContext>(app.ApplicationServices).GetAwaiter().GetResult();
        SeedDataAsync(app.ApplicationServices).GetAwaiter().GetResult();
        return app;
    }

    private static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userContextProvider = scope.ServiceProvider.GetRequiredService< IUserContextProvider>();
        userContextProvider.ClientId = "DataSeeder";
        userContextProvider.ClientIp = "DataSeeder";
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
        foreach (var seeder in seeders)
        {
            await seeder.SeedAllAsync();
        }
    }

    private static async Task MigrateDatabaseAsync<TContext>(IServiceProvider serviceProvider) where TContext : DbContext
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        await context.Database.MigrateAsync();
    }
}

