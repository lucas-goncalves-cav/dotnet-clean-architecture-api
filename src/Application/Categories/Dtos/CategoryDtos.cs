namespace CleanArchitecture.Application.Categories.Dtos;

public sealed record CreateCategoryRequest(string Name, string? Description);

public sealed record UpdateCategoryRequest(string Name, string? Description);

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string? Description,
    bool Active,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
