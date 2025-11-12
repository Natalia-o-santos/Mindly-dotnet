using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Mindly.Infrastructure.Persistence;

public class MindlyDbContextFactory : IDesignTimeDbContextFactory<MindlyDbContext>
{
    public MindlyDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MindlyDbContext>();
        var connectionString = configuration.GetConnectionString("Mindly") ?? "Data Source=mindly.db";

        optionsBuilder.UseSqlite(connectionString);

        return new MindlyDbContext(optionsBuilder.Options);
    }
}

