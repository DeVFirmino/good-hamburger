using GoodHamburger.Application.Abstractions;
using GoodHamburger.Application.Menu.Dtos;

namespace GoodHamburger.Application.Menu;

public sealed class MenuService : IMenuService
{
    private readonly IMenuItemRepository _menu;

    public MenuService(IMenuItemRepository menu)
    {
        _menu = menu;
    }

    public async Task<IReadOnlyList<MenuItemResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        var items = await _menu.ListAsync(cancellationToken);

        return items.Select(i => new MenuItemResponse
        {
            Id = i.Id,
            Name = i.Name,
            Category = i.Category,
            Price = i.Price
        }).ToList();
    }
}
