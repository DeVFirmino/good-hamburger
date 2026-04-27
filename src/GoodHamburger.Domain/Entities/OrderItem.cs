namespace GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
public class OrderItem
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public string? Name { get; set; }
    public MenuCategory Category { get; set; }
    public decimal Price { get; set; }
}