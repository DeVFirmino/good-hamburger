using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Services;

public class DiscountCalculator
{
    private const decimal FullComboRate = 0.20m;
    private const decimal SandwichDrinkRate = 0.15m;
    private const decimal SandwichFriesRate = 0.10m;
    private const decimal NoDiscount = 0m;

    public decimal Calculate(IReadOnlyCollection<OrderItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        bool hasSandwich = items.Any(i => i.Category == MenuCategory.Sandwich);
        bool hasFries    = items.Any(i => i.Category == MenuCategory.Fries);
        bool hasDrink    = items.Any(i => i.Category == MenuCategory.Drink);

        if (hasSandwich && hasFries && hasDrink) return FullComboRate;
        if (hasSandwich && hasDrink)             return SandwichDrinkRate;
        if (hasSandwich && hasFries)             return SandwichFriesRate;

        return NoDiscount;
    }
}
