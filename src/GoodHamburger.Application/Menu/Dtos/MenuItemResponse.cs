using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.Menu.Dtos;

public sealed class MenuItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public MenuCategory Category { get; set; }
    public decimal Price { get; set; }
}
