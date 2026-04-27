namespace GoodHamburger.Application.Orders.Dtos;

public sealed class OrderResponse
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal Total { get; set; }
}
