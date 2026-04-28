namespace GoodHamburger.Application.Orders.Dtos;

public sealed class CreateOrderRequest
{
    public List<int> MenuItemIds { get; set; } = new();
}
