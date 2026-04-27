using GoodHamburger.Application.Menu.Dtos;

namespace GoodHamburger.Application.Menu;

public interface IMenuService
{
    Task<IReadOnlyList<MenuItemResponse>> ListAsync(CancellationToken cancellationToken = default);
}
