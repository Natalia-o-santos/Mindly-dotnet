using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mindly.Domain.Abstractions;
using Mindly.Infrastructure.Persistence;
using Mindly.Infrastructure.Repositories;

namespace Mindly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Mindly") ?? "Data Source=mindly.db";

        services.AddDbContext<MindlyDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IFocusSessionRepository, FocusSessionRepository>();
        services.AddScoped<IDeviceSignalRepository, DeviceSignalRepository>();

        return services;
    }
}

