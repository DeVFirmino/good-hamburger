using FluentValidation;
using GoodHamburger.Application.Menu;
using GoodHamburger.Application.Orders;
using GoodHamburger.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<DiscountCalculator>();

        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IMenuService, MenuService>();

        services.AddValidatorsFromAssemblyContaining<IOrderService>();

        return services;
    }
}
