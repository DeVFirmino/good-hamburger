

namespace GoodHamburger.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    public void AddItem(MenuItem menuItem)
    {
        if (menuItem is null)
            throw new ArgumentNullException(nameof(menuItem));

        bool alreadyHasCategory = Items.Any(i => i.Category == menuItem.Category);
        if (alreadyHasCategory)
            throw new InvalidOperationException("Duplicate category");
        // TODO - throw new DuplicateCategoryException(menuItem.Category);
        // placeholder

        Items.Add(new OrderItem
        {
            MenuItemId = menuItem.Id,
            Name = menuItem.Name,
            Category = menuItem.Category,
            Price = menuItem.Price
        });
    }
}