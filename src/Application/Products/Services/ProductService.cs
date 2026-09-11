using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Products.Dtos;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Repositories;

namespace CleanArchitecture.Application.Products.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductResponse>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        if (!await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken))
        {
            return Result.Failure<ProductResponse>(Error.Validation($"Category {request.CategoryId} does not exist."));
        }

        var product = new Product(request.Name, request.Description, request.Price, request.CategoryId);

        await _productRepository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(product));
    }

    public async Task<Result<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductResponse>(Error.NotFound($"Product {id} was not found."));
        }

        if (!await _categoryRepository.ExistsAsync(request.CategoryId, cancellationToken))
        {
            return Result.Failure<ProductResponse>(Error.Validation($"Category {request.CategoryId} does not exist."));
        }

        product.Update(request.Name, request.Description, request.Price, request.CategoryId);
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Map(product));
    }

    public async Task<Result<ProductResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        return product is null
            ? Result.Failure<ProductResponse>(Error.NotFound($"Product {id} was not found."))
            : Result.Success(Map(product));
    }

    public async Task<Result<PagedResponse<ProductResponse>>> SearchAsync(ProductQuery query, CancellationToken cancellationToken = default)
    {
        var filter = new ProductFilter(
            query.Name,
            query.CategoryId,
            query.Active,
            query.SortBy,
            query.SortDescending,
            query.Page,
            query.PageSize);

        var page = await _productRepository.SearchAsync(filter, cancellationToken);

        var response = new PagedResponse<ProductResponse>(
            page.Items.Select(Map).ToList(),
            page.TotalItems,
            page.Page,
            page.PageSize,
            page.TotalPages,
            page.HasPrevious,
            page.HasNext);

        return Result.Success(response);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return Result.Failure(Error.NotFound($"Product {id} was not found."));
        }

        product.Deactivate();
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static ProductResponse Map(Product product) => new(
        product.Id,
        product.Name,
        product.Description,
        product.Price,
        product.CategoryId,
        product.Category?.Name,
        product.Active,
        product.CreatedAt,
        product.UpdatedAt);
}
