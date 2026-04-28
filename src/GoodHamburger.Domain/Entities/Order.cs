using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Services;

namespace GoodHamburger.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    public void AddItem(MenuItem menuItem)
    {
        ArgumentNullException.ThrowIfNull(menuItem);

        bool alreadyHasCategory = Items.Any(i => i.Category == menuItem.Category);
        if (alreadyHasCategory)
            throw new DuplicateCategoryException(menuItem.Category);

        Items.Add(new OrderItem
        {
            MenuItemId = menuItem.Id,
            Name       = menuItem.Name,
            Category   = menuItem.Category,
            Price      = menuItem.Price
        });
    }

    public decimal Subtotal() => Items.Sum(i => i.Price);

    public decimal DiscountRate(DiscountCalculator calculator)
    {
        ArgumentNullException.ThrowIfNull(calculator);
        return calculator.Calculate(Items);
    }

    public decimal Total(DiscountCalculator calculator)
    {
        decimal subtotal = Subtotal();
        decimal rate = DiscountRate(calculator);
        return subtotal - (subtotal * rate);
    }
}
