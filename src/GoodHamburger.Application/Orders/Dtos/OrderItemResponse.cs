using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.Orders.Dtos;

public sealed class OrderItemResponse
{
    public int MenuItemId { get; set; }
    public string Name { get; set; } = string.Empty;
    public MenuCategory Category { get; set; }
    public decimal Price { get; set; }
}
