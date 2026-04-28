using GoodHamburger.Application.Abstractions;
using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Persistence.Repositories;

public sealed class MenuItemRepository : IMenuItemRepository
{
    private readonly AppDbContext _db;

    public MenuItemRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.MenuItems.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<MenuItem>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _db.MenuItems
            .OrderBy(m => m.Category)
            .ThenBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }
}
