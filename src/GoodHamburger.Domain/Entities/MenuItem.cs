namespace GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

public class MenuItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public MenuCategory Category { get; set; }
    public decimal Price { get; set; }
}