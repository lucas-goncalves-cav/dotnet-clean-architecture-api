using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.Entities;

public class Category : BaseEntity
{
    private readonly List<Product> _products = [];

    private Category()
    {
    }

    public Category(string name, string? description)
    {
        SetName(name);
        Description = description;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    public void Update(string name, string? description)
    {
        SetName(name);
        Description = description;
        Touch();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Category name is required.");
        }

        Name = name.Trim();
    }
}
