using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.Products.Dtos;

namespace CleanArchitecture.Application.Products.Services;

public interface IProductService
{
    Task<Result<ProductResponse>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);

    Task<Result<ProductResponse>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);

    Task<Result<ProductResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<PagedResponse<ProductResponse>>> SearchAsync(ProductQuery query, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
