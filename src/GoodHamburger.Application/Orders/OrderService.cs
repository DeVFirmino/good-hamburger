using GoodHamburger.Application.Abstractions;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Application.Orders.Dtos;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Services;

namespace GoodHamburger.Application.Orders;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _orders;
    private readonly IMenuItemRepository _menu;
    private readonly IUnitOfWork _uow;
    private readonly DiscountCalculator _calculator;

    public OrderService(
        IOrderRepository orders,
        IMenuItemRepository menu,
        IUnitOfWork uow,
        DiscountCalculator calculator)
    {
        _orders = orders;
        _menu = menu;
        _uow = uow;
        _calculator = calculator;
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        await AddItemsAsync(order, request.MenuItemIds, cancellationToken);

        await _orders.AddAsync(order, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Map(order);
    }

    public async Task<OrderResponse> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken)
                    ?? throw new OrderNotFoundException(id);

        order.Items.Clear();
        await AddItemsAsync(order, request.MenuItemIds, cancellationToken);

        await _uow.SaveChangesAsync(cancellationToken);

        return Map(order);
    }

    public async Task<OrderResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken)
                    ?? throw new OrderNotFoundException(id);

        return Map(order);
    }

    public async Task<IReadOnlyList<OrderResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orders.ListAsync(cancellationToken);
        return orders.Select(Map).ToList();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orders.GetByIdAsync(id, cancellationToken)
                    ?? throw new OrderNotFoundException(id);

        _orders.Remove(order);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    private async Task AddItemsAsync(Order order, IEnumerable<int> menuItemIds, CancellationToken cancellationToken)
    {
        foreach (int menuItemId in menuItemIds)
        {
            var menuItem = await _menu.GetByIdAsync(menuItemId, cancellationToken)
                           ?? throw new InvalidMenuItemException(menuItemId);

            order.AddItem(menuItem);
        }
    }

    private OrderResponse Map(Order order) => new()
    {
        Id = order.Id,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(i => new OrderItemResponse
        {
            MenuItemId = i.MenuItemId,
            Name = i.Name,
            Category = i.Category,
            Price = i.Price
        }).ToList(),
        Subtotal = order.Subtotal(),
        DiscountRate = order.DiscountRate(_calculator),
        Total = order.Total(_calculator)
    };
}
