using GoodHamburger.Application.Abstractions;
using GoodHamburger.Infrastructure.Persistence;
using GoodHamburger.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        return services.AddInfrastructure(options => options.UseSqlite(connectionString));
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<AppDbContext>(configureDb);

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services;
    }
}
