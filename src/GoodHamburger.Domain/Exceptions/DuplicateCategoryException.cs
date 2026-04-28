using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Exceptions;

public sealed class DuplicateCategoryException : DomainException
{
    public MenuCategory Category { get; }

    public DuplicateCategoryException(MenuCategory category)
        : base("ORDER_DUPLICATE_CATEGORY",
               $"Order already has a {category}. Only one item per category is allowed.")
    {
        Category = category;
    }
}
