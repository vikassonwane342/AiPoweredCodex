using AiPoweredCodex.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AiPoweredCodex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<CarService>();
        services.AddScoped<BidService>();
        services.AddScoped<AdminService>();
        return services;
    }
}
