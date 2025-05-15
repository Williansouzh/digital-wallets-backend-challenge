using DigitalWallets.Domain.Account;
using DigitalWallets.Domain.Interfaces.Services;
using DigitalWallets.Infra.Data.Context;
using DigitalWallets.Infra.Data.Identity;
using DigitalWallets.Infra.Data.Persistence;
using DigitalWallets.Infra.Data.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalWallets.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //DbContext with Postgress Config
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b =>
                b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
        //Identity Config
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();
        //Register repositories
        services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthenticate, AuthenticateService>();
        //Register Unit Of Work 
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
