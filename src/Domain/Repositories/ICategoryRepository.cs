using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Category>> GetAllAsync(bool? active, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Category category, CancellationToken cancellationToken = default);

    void Update(Category category);
}
