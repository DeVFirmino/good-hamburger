using GoodHamburger.Application.Menu;
using GoodHamburger.Application.Menu.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MenuController : ControllerBase
{
    private readonly IMenuService _menu;

    public MenuController(IMenuService menu)
    {
        _menu = menu;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MenuItemResponse>>> List(CancellationToken cancellationToken)
    {
        var items = await _menu.ListAsync(cancellationToken);
        return Ok(items);
    }
}
