using Microsoft.EntityFrameworkCore;
using Mindly.Infrastructure.Persistence;
using Mindly.Infrastructure.Persistence.Seed;

namespace Mindly.Api.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MindlyDbContext>();

        await dbContext.Database.MigrateAsync();

        var seeder = new DataSeeder(dbContext);
        await seeder.SeedAsync();
    }
}

