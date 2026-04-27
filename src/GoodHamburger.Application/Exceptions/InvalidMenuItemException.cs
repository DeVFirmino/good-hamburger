using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.Application.Exceptions;

public sealed class InvalidMenuItemException : DomainException
{
    public int MenuItemId { get; }

    public InvalidMenuItemException(int menuItemId)
        : base("MENU_ITEM_NOT_FOUND", $"Menu item '{menuItemId}' does not exist.")
    {
        MenuItemId = menuItemId;
    }
}
