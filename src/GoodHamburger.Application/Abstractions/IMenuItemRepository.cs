using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Abstractions;

public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MenuItem>> ListAsync(CancellationToken cancellationToken = default);
}
