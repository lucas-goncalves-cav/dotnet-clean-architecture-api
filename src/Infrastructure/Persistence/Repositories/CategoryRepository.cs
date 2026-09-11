using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Category>> GetAllAsync(bool? active, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsNoTracking().AsQueryable();

        if (active.HasValue)
        {
            query = query.Where(x => x.Active == active.Value);
        }

        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Categories.AnyAsync(x => x.Id == id && x.Active, cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default) =>
        await _context.Categories.AddAsync(category, cancellationToken);

    public void Update(Category category) => _context.Categories.Update(category);
}
