using CleanArchitecture.Application.Categories.Dtos;
using CleanArchitecture.Application.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;

namespace CleanArchitecture.Application.Categories.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = new Category(request.Name, request.Description);

        await _repository.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(category));
    }

    public async Task<Result<CategoryResponse>> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(id, cancellationToken);

        if (category is null)
        {
            return Result.Failure<CategoryResponse>(Error.NotFound($"Category {id} was not found."));
        }

        category.Update(request.Name, request.Description);
        _repository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(category));
    }

    public async Task<Result<CategoryResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(id, cancellationToken);

        return category is null
            ? Result.Failure<CategoryResponse>(Error.NotFound($"Category {id} was not found."))
            : Result.Success(Map(category));
    }

    public async Task<Result<IReadOnlyCollection<CategoryResponse>>> GetAllAsync(bool? active, CancellationToken cancellationToken = default)
    {
        var categories = await _repository.GetAllAsync(active, cancellationToken);
        IReadOnlyCollection<CategoryResponse> response = categories.Select(Map).ToList();

        return Result.Success(response);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _repository.GetByIdAsync(id, cancellationToken);

        if (category is null)
        {
            return Result.Failure(Error.NotFound($"Category {id} was not found."));
        }

        category.Deactivate();
        _repository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static CategoryResponse Map(Category category) => new(
        category.Id,
        category.Name,
        category.Description,
        category.Active,
        category.CreatedAt,
        category.UpdatedAt);
}
