using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<PagedResult<Product>> SearchAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .Include(x => x.Category)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(x => x.Name.Contains(filter.Name));
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == filter.CategoryId.Value);
        }

        if (filter.Active.HasValue)
        {
            query = query.Where(x => x.Active == filter.Active.Value);
        }

        query = ApplySorting(query, filter);

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, totalItems, filter.Page, filter.PageSize);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) =>
        await _context.Products.AddAsync(product, cancellationToken);

    public void Update(Product product) => _context.Products.Update(product);

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, ProductFilter filter)
    {
        return filter.SortBy?.ToLowerInvariant() switch
        {
            "price" => filter.SortDescending ? query.OrderByDescending(x => x.Price) : query.OrderBy(x => x.Price),
            "createdat" => filter.SortDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
            _ => filter.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name)
        };
    }
}
