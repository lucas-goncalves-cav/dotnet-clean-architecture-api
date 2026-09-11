namespace CleanArchitecture.Application.Products.Dtos;

public sealed record CreateProductRequest(string Name, string? Description, decimal Price, Guid CategoryId);

public sealed record UpdateProductRequest(string Name, string? Description, decimal Price, Guid CategoryId);

public sealed record ProductQuery
{
    public string? Name { get; init; }

    public Guid? CategoryId { get; init; }

    public bool? Active { get; init; }

    public string? SortBy { get; init; }

    public bool SortDescending { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    string? CategoryName,
    bool Active,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
