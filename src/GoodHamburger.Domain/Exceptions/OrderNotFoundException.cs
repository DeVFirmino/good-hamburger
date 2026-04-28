namespace GoodHamburger.Domain.Exceptions;

public sealed class OrderNotFoundException : DomainException
{
    public Guid OrderId { get; }

    public OrderNotFoundException(Guid id)
        : base("ORDER_NOT_FOUND", $"Order '{id}' was not found.")
    {
        OrderId = id;
    }
}
