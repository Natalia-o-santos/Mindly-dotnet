using Microsoft.Extensions.DependencyInjection;
using Mindly.Application.Services;
using Mindly.Application.Services.Contracts;

namespace Mindly.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IFocusSessionService, FocusSessionService>();

        return services;
    }
}

