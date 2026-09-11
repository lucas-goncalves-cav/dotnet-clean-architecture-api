using CleanArchitecture.Application.Categories.Dtos;
using CleanArchitecture.Application.Common;

namespace CleanArchitecture.Application.Categories.Services;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);

    Task<Result<CategoryResponse>> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);

    Task<Result<CategoryResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyCollection<CategoryResponse>>> GetAllAsync(bool? active, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
