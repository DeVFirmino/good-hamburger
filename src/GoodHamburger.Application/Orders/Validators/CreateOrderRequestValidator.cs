using FluentValidation;
using GoodHamburger.Application.Orders.Dtos;

namespace GoodHamburger.Application.Orders.Validators;

public sealed class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.MenuItemIds)
            .NotNull().WithMessage("MenuItemIds is required.")
            .Must(ids => ids.Count > 0).WithMessage("Order must contain at least one item.")
            .Must(ids => ids.Count <= 3).WithMessage("Order can contain at most three items (one per category).");

        RuleForEach(x => x.MenuItemIds)
            .GreaterThan(0).WithMessage("MenuItemId must be a positive number.");
    }
}
