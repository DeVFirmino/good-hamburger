namespace GoodHamburger.Application.Orders.Dtos;

public sealed class UpdateOrderRequest
{
    public List<int> MenuItemIds { get; set; } = new();
}
