using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Exceptions;

namespace CleanArchitecture.Domain.Entities;

public class Product : BaseEntity
{
    private Product()
    {
    }

    public Product(string name, string? description, decimal price, Guid categoryId)
    {
        SetName(name);
        SetPrice(price);
        Description = description;
        CategoryId = categoryId;
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public decimal Price { get; private set; }

    public Guid CategoryId { get; private set; }

    public Category? Category { get; private set; }

    public void Update(string name, string? description, decimal price, Guid categoryId)
    {
        SetName(name);
        SetPrice(price);
        Description = description;
        CategoryId = categoryId;
        Touch();
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name is required.");
        }

        Name = name.Trim();
    }

    private void SetPrice(decimal price)
    {
        if (price <= 0)
        {
            throw new DomainException("Product price must be greater than zero.");
        }

        Price = price;
    }
}
