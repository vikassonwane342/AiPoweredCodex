using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Application.Abstractions.Security;
using AiPoweredCodex.Domain.Entities;
using AiPoweredCodex.Domain.Enums;
using AiPoweredCodex.Infrastructure.Persistence;
using AiPoweredCodex.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiPoweredCodex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddSingleton<DatabaseInitializer>();
        services.AddScoped<IUserRepository, SqlUserRepository>();
        services.AddScoped<ICarRepository, SqlCarRepository>();
        services.AddScoped<IBidRepository, SqlBidRepository>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenService, HmacTokenService>();

        services.AddScoped<DemoDataSeeder>();
        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync();
    }

    public static async Task SeedDemoDataAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DemoDataSeeder>();
        await seeder.SeedAsync();
    }
}

public sealed class DemoDataSeeder
{
    private readonly IUserRepository _userRepository;
    private readonly ICarRepository _carRepository;
    private readonly IPasswordHasher _passwordHasher;

    public DemoDataSeeder(IUserRepository userRepository, ICarRepository carRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _carRepository = carRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        var admin = await _userRepository.GetByEmailAsync("admin@aipoweredcodex.com", CancellationToken.None);
        if (admin is not null)
        {
            return;
        }

        var adminUser = new AppUser("Platform Admin", "admin@aipoweredcodex.com", _passwordHasher.Hash("Admin@123"), UserRole.Admin);
        var seller = new AppUser("Demo Seller", "seller@aipoweredcodex.com", _passwordHasher.Hash("Seller@123"), UserRole.Seller);
        var buyer = new AppUser("Demo Buyer", "buyer@aipoweredcodex.com", _passwordHasher.Hash("Buyer@123"), UserRole.User);

        await _userRepository.AddAsync(adminUser, CancellationToken.None);
        await _userRepository.AddAsync(seller, CancellationToken.None);
        await _userRepository.AddAsync(buyer, CancellationToken.None);

        var car = new Car(seller.Id, "2022 Hyundai Creta SX", "Hyundai", "Creta", 2022, 1450000m, "Single-owner SUV with service history and automatic transmission.");
        await _carRepository.AddAsync(car, CancellationToken.None);
    }
}
