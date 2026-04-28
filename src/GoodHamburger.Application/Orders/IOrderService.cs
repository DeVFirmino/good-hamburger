using GoodHamburger.Application.Orders.Dtos;

namespace GoodHamburger.Application.Orders;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderResponse>> ListAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
