namespace CleanArchitecture.Domain.Repositories;

public sealed record ProductFilter(
    string? Name,
    Guid? CategoryId,
    bool? Active,
    string? SortBy,
    bool SortDescending,
    int Page,
    int PageSize);
