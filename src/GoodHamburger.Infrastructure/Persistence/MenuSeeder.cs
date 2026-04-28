using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Persistence;

public static class MenuSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.MenuItems.AnyAsync(cancellationToken))
            return;

        var items = new[]
        {
            new MenuItem { Name = "X Burger",     Category = MenuCategory.Sandwich, Price = 5.00m },
            new MenuItem { Name = "X Egg",        Category = MenuCategory.Sandwich, Price = 4.50m },
            new MenuItem { Name = "X Bacon",      Category = MenuCategory.Sandwich, Price = 7.00m },
            new MenuItem { Name = "Batata frita", Category = MenuCategory.Fries,    Price = 2.00m },
            new MenuItem { Name = "Refrigerante", Category = MenuCategory.Drink,    Price = 2.50m }
        };

        await db.MenuItems.AddRangeAsync(items, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }
}
